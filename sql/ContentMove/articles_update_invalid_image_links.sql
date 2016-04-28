UPDATE ContentItems
SET Description = REPLACE(REPLACE(REPLACE(c.Description, 'http://www.vitalchoice.com/shop/pc/catalog/', '/files/catalog/'), '/shop/pc/catalog/', '/files/catalog/'), '../pc/catalog/', '/files/catalog/') 
FROM ContentItems AS c
INNER JOIN Articles AS a ON a.ContentItemId = c.Id
WHERE c.Description LIKE '%/catalog/%'