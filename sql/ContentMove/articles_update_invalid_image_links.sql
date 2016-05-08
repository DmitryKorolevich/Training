UPDATE Articles
--SET Description = REPLACE(REPLACE(REPLACE(c.Description, 'http://www.vitalchoice.com/shop/pc/catalog/', '/files/catalog/'), '/shop/pc/catalog/', '/files/catalog/'), '../pc/catalog/', '/files/catalog/') 
SET FileUrl = REPLACE(REPLACE(REPLACE(c.FileUrl, 'http://www.vitalchoice.com/shop/pc/catalog/', '/files/catalog/'), '/shop/pc/catalog/', '/files/catalog/'), '../pc/catalog/', '/files/catalog/')
FROM Articles AS c
WHERE c.FileUrl LIKE '%/catalog/%'