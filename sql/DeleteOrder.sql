DECLARE @orderId int, @orderPaymentMethodId int

SET @orderId=9

SET @orderPaymentMethodId= (SELECT TOP 1 IdPaymentMethod FROM Orders
WHERE Id=@orderId)

DELETE OrderOptionValues
WHERE IdOrder=@orderId

DELETE OrderToSkus
WHERE IdOrder=@orderId

DELETE Orders
WHERE Id=@orderId

DELETE OrderPaymentMethodOptionValues
WHERE IdOrderPaymentMethod=@orderPaymentMethodId

DELETE OrderPaymentMethods
WHERE Id=@orderPaymentMethodId