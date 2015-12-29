GO

IF NOT EXISTS(SELECT * 
FROM sys.indexes 
WHERE name='IX_Articles_Url_StatusCode' AND object_id = OBJECT_ID('Articles'))
BEGIN

CREATE NONCLUSTERED INDEX [IX_Articles_Url_StatusCode]
    ON [dbo].[Articles]([Url],[StatusCode]) WITH (FILLFACTOR = 80);
CREATE NONCLUSTERED INDEX [IX_Recipes_Url_StatusCode]
    ON [dbo].[Recipes]([Url],[StatusCode]) WITH (FILLFACTOR = 80);
CREATE NONCLUSTERED INDEX [IX_FAQs_Url_StatusCode]
    ON [dbo].[FAQs]([Url],[StatusCode]) WITH (FILLFACTOR = 80);
CREATE NONCLUSTERED INDEX [IX_ContentPages_Url_StatusCode]
    ON [dbo].[ContentPages]([Url],[StatusCode]) WITH (FILLFACTOR = 80);
CREATE NONCLUSTERED INDEX [IX_Products_Url_StatusCode]
    ON [dbo].[Products]([Url],[StatusCode]) WITH (FILLFACTOR = 80);
CREATE NONCLUSTERED INDEX [IX_ProductCategories_Url_StatusCode]
    ON [dbo].[ProductCategories]([Url],[StatusCode]) WITH (FILLFACTOR = 80);
CREATE NONCLUSTERED INDEX [IX_ContentCategories_Url_StatusCode]
    ON [dbo].[ContentCategories]([Type],[Url],[StatusCode]) WITH (FILLFACTOR = 80);

END

GO