using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Transaction;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.VeraCore;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using VitalChoice.Business.Mail;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Entities.VeraCore;
using VitalChoice.Ecommerce.Domain.Entities.VeraCore.FilesSchema;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.VeraCore;
using VitalChoice.Interfaces.Services.Avatax;
using VitalChoice.Interfaces.Services.Orders;

namespace VitalChoice.Business.Services.VeraCore
{
    public class VeraCoreNotificationService : IVeraCoreNotificationService
    {
        private readonly IOptions<AppOptions> _options;
        private readonly IEcommerceRepositoryAsync<VeraCoreProcessItem> _veraCoreProcessItemRepository;
        private readonly IEcommerceRepositoryAsync<VeraCoreProcessLogItem> _veraCoreProcessLogItemRepository;
        private readonly IEcommerceRepositoryAsync<Order> _orderRepository;
        private readonly IVeraCoreSFTPService _veraCoreSFTPService;
        private readonly IVeraCoreFilesCacheService _veraCoreFilesCacheService;
        private readonly IOrderService _orderService;
        private readonly IOrderRefundService _orderRefundService;
        private readonly ITransactionAccessor<EcommerceContext> _transactionEcommerceAccessor;
        private readonly INotificationService _notificationService;
        private readonly IAvalaraTax _avalaraTax;
        private readonly OrderMapper _orderMapper;
        private readonly Regex _shipFileNamePattern;
        private readonly Regex _cancelFileNamePattern;
        private readonly Regex _numberPattern;
        private readonly ILogger _logger;

        public VeraCoreNotificationService(
            IOptions<AppOptions> options,
            IEcommerceRepositoryAsync<VeraCoreProcessItem> veraCoreProcessItemRepository,
            IEcommerceRepositoryAsync<VeraCoreProcessLogItem> veraCoreProcessLogItemRepository,
            IEcommerceRepositoryAsync<Order> orderRepository,
            IVeraCoreSFTPService veraCoreSFTPService,
            IVeraCoreFilesCacheService veraCoreFilesCacheService,
            IOrderService orderService,
            IOrderRefundService orderRefundService,
            ITransactionAccessor<EcommerceContext> transactionEcommerceAccessor,
            INotificationService notificationService,
            IAvalaraTax avalaraTax,
            OrderMapper orderMapper,
            ILoggerProviderExtended logger)
        {
            _options = options;
            _veraCoreProcessItemRepository = veraCoreProcessItemRepository;
            _veraCoreProcessLogItemRepository = veraCoreProcessLogItemRepository;
            _orderRepository = orderRepository;
            _veraCoreSFTPService = veraCoreSFTPService;
            _veraCoreFilesCacheService = veraCoreFilesCacheService;
            _orderService = orderService;
            _orderRefundService = orderRefundService;
            _transactionEcommerceAccessor = transactionEcommerceAccessor;
            _shipFileNamePattern = new Regex(VeraCoreConstants.ShipPattern, RegexOptions.Compiled);
            _cancelFileNamePattern = new Regex(VeraCoreConstants.CancelPattern, RegexOptions.Compiled);
            _numberPattern = new Regex("([0-9]+)", RegexOptions.Compiled);
            _notificationService = notificationService;
            _avalaraTax = avalaraTax;
            _orderMapper = orderMapper;
            _logger = logger.CreateLogger<VeraCoreSFTPService>();
        }


        public async Task Process()
        {
            await ProcessFiles();
            await ProcessQueue();
            _veraCoreFilesCacheService.MakeBackup();

        }

        public async Task ProcessFiles()
        {
            _logger.LogWarning("Initiating FTP Scan");
            try
            {
                var fileList = _veraCoreSFTPService.GetFileList(VeraCoreSFTPOptions.Export);
                _logger.LogWarning("List Done");

                using (var uow = _transactionEcommerceAccessor.CreateUnitOfWork())
                {
                    var localVeraCoreProcessItemRepository = uow.RepositoryAsync<VeraCoreProcessItem>();
                    var localVeraCoreProcessLogItemRepository = uow.RepositoryAsync<VeraCoreProcessLogItem>();

                    //Do not process again the same files
                    var fileNames = fileList.Select(p => p.FileName).ToList();
                    fileNames = fileNames.Where(p => _shipFileNamePattern.IsMatch(p) || _cancelFileNamePattern.IsMatch(p)).ToList();
                    var logItems = await localVeraCoreProcessLogItemRepository.Query(p => fileNames.Contains(p.FileName)).SelectAsync(false);
                    foreach (var logItem in logItems)
                    {
                        var file = fileList.FirstOrDefault(p => p.FileName == logItem.FileName);
                        if (file != null && file.FileDate == logItem.FileDate && file.FileSize == logItem.FileSize)
                        {
                            fileList.Remove(file);
                            //And remove from source
                            _veraCoreSFTPService.RemoveFile(file.FileName);
                        }
                    }

                    _logger.LogWarning("Downloading files");
                    var files = new List<VeraCoreFile>();
                    foreach (var fileName in fileNames)
                    {
                        var fileInfo = fileList.FirstOrDefault(p => p.FileName == fileName);
                        if (fileInfo != null)
                        {
                            var file = CacheFile(fileInfo);
                            if (file != null)
                            {
                                files.Add(file);
                            }
                        }
                    }

                    var activeItems = new List<VeraCoreProcessItem>();
                    var newLogItems = new List<VeraCoreProcessLogItem>();
                    files.ForEach(p =>
                    {
                        var activeItem = new VeraCoreProcessItem()
                        {
                            Attempt = 0,
                            Data = p.Data,
                            FileName = p.FileName,
                            FileSize = p.FileSize,
                            FileDate = p.FileDate,
                            DateCreated = DateTime.Now,
                        };
                        if (_shipFileNamePattern.IsMatch(p.FileName))
                        {
                            activeItem.IdType = VeraCoreProcessItemType.Ship;
                        }
                        if (_cancelFileNamePattern.IsMatch(p.FileName))
                        {
                            activeItem.IdType = VeraCoreProcessItemType.Cancel;
                        }
                        activeItems.Add(activeItem);

                        var logItem = new VeraCoreProcessLogItem()
                        {
                            FileName = p.FileName,
                            FileSize = p.FileSize,
                            FileDate = p.FileDate,
                            DateCreated = DateTime.Now,
                        };
                        newLogItems.Add(logItem);
                    });

                    await localVeraCoreProcessItemRepository.InsertRangeAsync(activeItems);
                    await localVeraCoreProcessLogItemRepository.InsertRangeAsync(newLogItems);
                    await uow.SaveChangesAsync();

                    foreach (var file in files)
                    {
                        _veraCoreSFTPService.RemoveFile(file.FileName);
                    }
                }

                _logger.LogWarning("SFTP files are processed succefully");
            }
            catch (Exception e)
            {
                _logger.LogError("SFTP files error");
                _logger.LogError(e.ToString());
            }
        }

        private VeraCoreFile CacheFile(VeraCoreFileInfo fileInfo)
        {
            VeraCoreFile toReturn = null;
            using (MemoryStream stream = _veraCoreSFTPService.DownloadFileData(fileInfo.FileName))
            {
                var result = _veraCoreFilesCacheService.CacheFile(fileInfo, stream,
                    _options.Value.VeraCoreSettings.ExportFolderName);
                if (result)
                {
                    toReturn = new VeraCoreFile(fileInfo, Encoding.ASCII.GetString(stream.ToArray()));
                }
                stream.Close();
            }
            return toReturn;
        }

        public async Task ProcessQueue()
        {
            try
            {
                var updated = false;

                var items = await _veraCoreProcessItemRepository.Query(p => p.Attempt < VeraCoreConstants.MAX_PARSING_ATTEMPT).SelectAsync(false);

                updated = await ProcessShipment(items.Where(p => p.IdType == VeraCoreProcessItemType.Ship).ToList());
                updated = await ProcessCancel(items.Where(p => p.IdType == VeraCoreProcessItemType.Cancel).ToList()) || updated;

                _logger.LogWarning(updated ? "Orders and Cache updated successfully" : "No updates in queue");
            }
            catch (Exception e)
            {
                _logger.LogError("Queue process error");
                _logger.LogError(e.ToString());
            }
        }

        private async Task<bool> ProcessShipment(ICollection<VeraCoreProcessItem> items)
        {
            var toReturn = false;

            var shipmentNotificationItems = await ProcessOrderShipmentPackagesUpdate(items);
            toReturn = shipmentNotificationItems.Count > 0;
            var orders = await _orderService.SelectAsync(shipmentNotificationItems.Select(p => p.IdOrder).ToList());
            await SendShipmentNotifications(shipmentNotificationItems, orders);
            CommitTaxes(shipmentNotificationItems, orders);

            return toReturn;
        }

        private void CommitTaxes(ICollection<ShipmentNotificationItem> shipmentNotificationItems, List<OrderDynamic> orders)
        {
            shipmentNotificationItems.AsParallel().ForAll(p =>
            {
                var order = orders.FirstOrDefault(pp => pp.Id == p.IdOrder);
                if (order != null)
                {
                    TaxGetType type;
                    var orderCode = _orderService.GenerateOrderCode(p.POrderType, order.Id, out type);
                    _avalaraTax.CommitTax(orderCode, order.ShippingAddress?.IdState, type).Wait();
                }
            });
        }

        private async Task SendShipmentNotifications(ICollection<ShipmentNotificationItem> shipmentNotificationItems, ICollection<OrderDynamic> orders)
        {
            if (shipmentNotificationItems.Count > 0)
            {
                var emailModels = new List<OrderShippingConfirmationEmail>();
                foreach (var veraCoreShipmentNotificationItem in shipmentNotificationItems)
                {
                    var order = orders.FirstOrDefault(p => p.Id == veraCoreShipmentNotificationItem.IdOrder);
                    if (order != null)
                    {
                        order.SendSide = (int?)veraCoreShipmentNotificationItem.POrderType;
                        var emailModel = await _orderMapper.ToModelAsync<OrderShippingConfirmationEmail>(order);
                        emailModel.ToEmail = emailModel.Email;
                        emailModels.Add(emailModel);
                    }
                }

                await _notificationService.SendOrderShippingConfirmationEmailsAsync(emailModels);
            }
        }

        private async Task<ICollection<ShipmentNotificationItem>> ProcessOrderShipmentPackagesUpdate(ICollection<VeraCoreProcessItem> items)
        {
            ICollection<ShipmentNotificationItem> toReturn = new List<ShipmentNotificationItem>();
            var serializer = new XmlSerializer(typeof(ShipmentNotice));
            var now = DateTime.Now;
            foreach (var item in items)
            {
                var parsed = true;
                int? orderId = null;
                int? pOrderType = null;
                try
                {
                    var data = Encoding.ASCII.GetBytes(item.Data);
                    using (MemoryStream stream = new MemoryStream(data))
                    {
                        var shipNotice = (ShipmentNotice)serializer.Deserialize(stream);
                        using (var uow = _transactionEcommerceAccessor.CreateUnitOfWork())
                        {
                            var packages = new List<OrderShippingPackage>();
                            
                            var skuRepository = uow.RepositoryAsync<Sku>();
                            var orderShippingPackageRepository = uow.RepositoryAsync<OrderShippingPackage>();
                            var orderRepository = uow.RepositoryAsync<Order>();
                            var veraCoreProcessItemRepository = uow.RepositoryAsync<VeraCoreProcessItem>();
                            foreach (PurchaseOrderInformation orderInfo in shipNotice.PurchaseOrderInformation)
                            {
                                string sNumber = orderInfo.PurchaseOrderNumber;
                                if (sNumber.Contains('-'))
                                {
                                    sNumber = orderInfo.PurchaseOrderNumber.Split(new[] { '-' })[0];
                                }
                                if (sNumber.EndsWith("_np"))
                                {
                                    pOrderType = (int)POrderType.NP;
                                    sNumber = sNumber.Replace("_np", "");
                                }
                                if (sNumber.EndsWith("_p"))
                                {
                                    pOrderType = (int)POrderType.P;
                                    sNumber = sNumber.Replace("_p", "");
                                }

                                if (_numberPattern.IsMatch(sNumber))
                                    orderId = Int32.Parse(sNumber);

                                if (!orderId.HasValue)
                                {
                                    _logger.LogError("Update notification - missed id order");
                                    parsed = false;
                                    break;
                                }

                                int pickSlipId;
                                int.TryParse(shipNotice.PickSlipId, out pickSlipId);
                                int? resultId = pickSlipId == 0 ? null : (int?)pickSlipId;

                                var skuCodes = orderInfo.ItemInformation.Select(p => p.VendorProductID).ToList();
                                var skus = await skuRepository.Query(p => skuCodes.Contains(p.Code)).SelectAsync(false);

                                var missedSku = false;
                                foreach (var itemInformation in orderInfo.ItemInformation)
                                {
                                    var sku = skus.FirstOrDefault(pp => pp.Code == itemInformation.VendorProductID);
                                    if (sku == null)
                                    {
                                        _logger.LogError("Update notification - missed sku code");
                                        missedSku = true;
                                        parsed = false;
                                        break;
                                    }

                                    OrderShippingPackage package = new OrderShippingPackage();
                                    package.IdOrder = orderId.Value;
                                    package.IdSku = sku.Id;
                                    package.POrderType = pOrderType;
                                    package.DateCreated = now;
                                    package.ShipMethodFreightCarrier = shipNotice.FreightCarrier;
                                    package.ShipMethodFreightService = shipNotice.FreightService;
                                    package.ShippedDate = shipNotice.ShipDate;
                                    package.TrackingNumber = itemInformation.LabelInformation.LabelTrackingNumber;
                                    package.UPSServiceCode = itemInformation.VendorProductID;
                                    package.IdWarehouse = resultId >= _options.Value.VeraCoreSettings.WAwarehouseThreshold
                                        ? Warehouse.WA
                                        : Warehouse.VA;

                                    packages.Add(package);
                                }
                                if (missedSku)
                                {
                                    break;
                                }
                            }

                            //Parsed without exceptions
                            if (parsed)
                            {
                                var order =
                                    (await orderRepository.Query(p => p.Id == orderId && p.StatusCode != (int)RecordStatusCode.Deleted).SelectAsync(true)).FirstOrDefault
                                        ();
                                if (order == null ||
                                    (!pOrderType.HasValue &&
                                     (!order.OrderStatus.HasValue || (order.OrderStatus != OrderStatus.Exported &&
                                                                      order.OrderStatus != OrderStatus.Shipped))) ||
                                    (pOrderType == (int) POrderType.P &&
                                     (!order.POrderStatus.HasValue || (order.POrderStatus != OrderStatus.Exported &&
                                                                       order.POrderStatus != OrderStatus.Shipped))) ||
                                    (pOrderType == (int) POrderType.NP &&
                                     (!order.NPOrderStatus.HasValue || (order.NPOrderStatus != OrderStatus.Exported &&
                                                                        order.NPOrderStatus != OrderStatus.Shipped))))
                                {
                                    _logger.LogError(
                                        $"Update notification - invalid order or shipment notification(order id - {orderId})");
                                    await IncrementAttempt(item);
                                    continue;
                                }

                                if (!pOrderType.HasValue)
                                {
                                    order.OrderStatus = OrderStatus.Shipped;
                                }
                                if (pOrderType == (int) POrderType.P)
                                {
                                    order.POrderStatus = OrderStatus.Shipped;
                                }
                                if (pOrderType == (int) POrderType.NP)
                                {
                                    order.NPOrderStatus = OrderStatus.Shipped;
                                }
                                await orderShippingPackageRepository.InsertRangeAsync(packages);
                                await veraCoreProcessItemRepository.DeleteAsync(item.Id);
                                await uow.SaveChangesAsync();

                                await _orderService.LogOrderUpdateAsync(order.Id);


                                toReturn.Add(new ShipmentNotificationItem()
                                {
                                    IdOrder = orderId.Value,
                                    POrderType = (POrderType?) pOrderType
                                });
                            }
                            else
                            {
                                await IncrementAttempt(item);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"VeraCore - item process error {orderId}");
                    _logger.LogError(e.ToString());
                    await IncrementAttempt(item);
                }
            }

            return toReturn;
        }

        private async Task IncrementAttempt(VeraCoreProcessItem item)
        {
            var processItem =
                (await _veraCoreProcessItemRepository.Query(p => p.Id == item.Id).SelectAsync()).FirstOrDefault();
            if (processItem != null)
            {
                processItem.Attempt++;
                await _veraCoreProcessItemRepository.UpdateAsync(processItem);
            }
        }

        private async Task<bool> ProcessCancel(ICollection<VeraCoreProcessItem> items)
        {
            var toReturn = false;
            var serializer = new XmlSerializer(typeof(CancelAck));
            foreach (var item in items)
            {
                var parsed = true;
                int? orderId = null;
                POrderType? pOrderType = null;
                try
                {
                    var data = Encoding.ASCII.GetBytes(item.Data);
                    using (MemoryStream stream = new MemoryStream(data))
                    {
                        var cancelNotice = (CancelAck)serializer.Deserialize(stream);
                        if (cancelNotice.CanceledComplete.ToLower() == "y")
                        {
                            string sNumber = cancelNotice.OrderID;
                            if (sNumber.EndsWith("_np"))
                            {
                                pOrderType = POrderType.NP;
                                sNumber = sNumber.Replace("_np", "");
                            }
                            if (sNumber.EndsWith("_p"))
                            {
                                pOrderType = POrderType.P;
                                sNumber = sNumber.Replace("_p", "");
                            }

                            if (_numberPattern.IsMatch(sNumber))
                                orderId = Int32.Parse(sNumber);

                            if (!orderId.HasValue)
                            {
                                _logger.LogError("Cancel notification - missed id order");
                                parsed = false;
                                break;
                            }

                            //Parsed without exceptions
                            if (parsed)
                            {
                                var order = (await _orderRepository.Query(p => p.Id == orderId && p.StatusCode!=(int)RecordStatusCode.Deleted)
                                    .SelectAsync(false)).FirstOrDefault();
                                if (order == null || (!pOrderType.HasValue && order.OrderStatus!=OrderStatus.Exported) ||
                                    (pOrderType==POrderType.P && order.POrderStatus != OrderStatus.Exported) ||
                                    (pOrderType == POrderType.NP && order.NPOrderStatus != OrderStatus.Exported))
                                {
                                    _logger.LogError(
                                        $"Update cancel - invalid order or cancel notification(order id - {orderId})");
                                    await IncrementAttempt(item);
                                    continue;
                                }

                                if (order.IdObjectType == (int) OrderType.Refund)
                                {
                                    await _orderRefundService.CancelRefundOrderAsync(order.Id);
                                }
                                else
                                {
                                    await _orderService.CancelOrderAsync(order.Id, pOrderType);
                                }

                                await _veraCoreProcessItemRepository.DeleteAsync(item.Id);

                                TaxGetType type;
                                var orderCode =_orderService.GenerateOrderCode(pOrderType, order.Id, out type);
                                await _avalaraTax.CancelTax(orderCode);
                            }

                            if (!parsed)
                            {
                                await IncrementAttempt(item);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"VeraCore - item process error {orderId}");
                    _logger.LogError(e.ToString());
                    await IncrementAttempt(item);
                }
            }

            return toReturn;
        }
    }
}