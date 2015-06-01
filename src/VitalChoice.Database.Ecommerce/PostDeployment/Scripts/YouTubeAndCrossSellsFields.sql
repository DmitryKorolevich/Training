GO

DECLARE @count INT

SET @count=(SELECT COUNT(*) FROM ProductOptionTypes WHERE Name='CrossSellImage1')

IF @count=0
BEGIN

	INSERT INTO ProductOptionTypes
	(DefaultValue, IdFieldType, IdProductType, Name)
	SELECT '/some1.png', 4, 1, N'CrossSellImage1'
	UNION	
	SELECT '/some2.png', 4, 1, N'CrossSellImage2'
	UNION
	SELECT '/some3.png', 4, 1, N'CrossSellImage3'
	UNION
	SELECT '/some4.png', 4, 1, N'CrossSellImage4'
	UNION
	SELECT 'http://someurl.com/1', 4, 1, N'CrossSellUrl1'
	UNION
	SELECT 'http://someurl.com/1', 4, 1, N'CrossSellUrl2'
	UNION
	SELECT 'http://someurl.com/1', 4, 1, N'CrossSellUrl3'
	UNION
	SELECT 'http://someurl.com/1', 4, 1, N'CrossSellUrl4'
	UNION
	SELECT '/some1.png', 4, 1, N'YouTubeImage1'
	UNION	
	SELECT '/some2.png', 4, 1, N'YouTubeImage2'
	UNION
	SELECT '/some3.png', 4, 1, N'YouTubeImage3'
	UNION
	SELECT '/some1.png', 4, 1, N'YouTubeText1'
	UNION	
	SELECT '/some2.png', 4, 1, N'YouTubeText2'
	UNION
	SELECT '/some3.png', 4, 1, N'YouTubeText3'
	UNION
	SELECT 'jGwOsFo8TTg', 4, 1, N'YouTubeVideo1'
	UNION	
	SELECT 'btlfoO75kfI', 4, 1, N'YouTubeVideo2'
	UNION
	SELECT 'vCsRTamxWuw', 4, 1, N'YouTubeVideo3'
	UNION
	SELECT '/some1.png', 4, 2, N'CrossSellImage1'
	UNION	
	SELECT '/some2.png', 4, 2, N'CrossSellImage2'
	UNION
	SELECT '/some3.png', 4, 2, N'CrossSellImage3'
	UNION
	SELECT '/some4.png', 4, 2, N'CrossSellImage4'
	UNION
	SELECT 'http://someurl.com/1', 4, 2, N'CrossSellUrl1'
	UNION
	SELECT 'http://someurl.com/1', 4, 2, N'CrossSellUrl2'
	UNION
	SELECT 'http://someurl.com/1', 4, 2, N'CrossSellUrl3'
	UNION
	SELECT 'http://someurl.com/1', 4, 2, N'CrossSellUrl4'
	UNION
	SELECT '/some1.png', 4, 2, N'YouTubeImage1'
	UNION	
	SELECT '/some2.png', 4, 2, N'YouTubeImage2'
	UNION
	SELECT '/some3.png', 4, 2, N'YouTubeImage3'
	UNION
	SELECT '/some1.png', 4, 2, N'YouTubeText1'
	UNION	
	SELECT '/some2.png', 4, 2, N'YouTubeText2'
	UNION
	SELECT '/some3.png', 4, 2, N'YouTubeText3'
	UNION
	SELECT 'jGwOsFo8TTg', 4, 2, N'YouTubeVideo1'
	UNION	
	SELECT 'btlfoO75kfI', 4, 2, N'YouTubeVideo2'
	UNION
	SELECT 'vCsRTamxWuw', 4, 2, N'YouTubeVideo3'
	UNION
	SELECT '/some1.png', 4, 3, N'CrossSellImage1'
	UNION	
	SELECT '/some2.png', 4, 3, N'CrossSellImage2'
	UNION
	SELECT '/some3.png', 4, 3, N'CrossSellImage3'
	UNION
	SELECT '/some4.png', 4, 3, N'CrossSellImage4'
	UNION
	SELECT 'http://someurl.com/1', 4, 3, N'CrossSellUrl1'
	UNION
	SELECT 'http://someurl.com/1', 4, 3, N'CrossSellUrl2'
	UNION
	SELECT 'http://someurl.com/1', 4, 3, N'CrossSellUrl3'
	UNION
	SELECT 'http://someurl.com/1', 4, 3, N'CrossSellUrl4'
	UNION
	SELECT '/some1.png', 4, 3, N'YouTubeImage1'
	UNION	
	SELECT '/some2.png', 4, 3, N'YouTubeImage2'
	UNION
	SELECT '/some3.png', 4, 3, N'YouTubeImage3'
	UNION
	SELECT '/some1.png', 4, 3, N'YouTubeText1'
	UNION	
	SELECT '/some2.png', 4, 3, N'YouTubeText2'
	UNION
	SELECT '/some3.png', 4, 3, N'YouTubeText3'
	UNION
	SELECT 'jGwOsFo8TTg', 4, 3, N'YouTubeVideo1'
	UNION	
	SELECT 'btlfoO75kfI', 4, 3, N'YouTubeVideo2'
	UNION
	SELECT 'vCsRTamxWuw', 4, 3, N'YouTubeVideo3'
	UNION
	SELECT '/some1.png', 4, 4, N'CrossSellImage1'
	UNION	
	SELECT '/some2.png', 4, 4, N'CrossSellImage2'
	UNION
	SELECT '/some3.png', 4, 4, N'CrossSellImage3'
	UNION
	SELECT '/some4.png', 4, 4, N'CrossSellImage4'
	UNION
	SELECT 'http://someurl.com/1', 4, 4, N'CrossSellUrl1'
	UNION
	SELECT 'http://someurl.com/1', 4, 4, N'CrossSellUrl2'
	UNION
	SELECT 'http://someurl.com/1', 4, 4, N'CrossSellUrl3'
	UNION
	SELECT 'http://someurl.com/1', 4, 4, N'CrossSellUrl4'
	UNION
	SELECT '/some1.png', 4, 4, N'YouTubeImage1'
	UNION	
	SELECT '/some2.png', 4, 4, N'YouTubeImage2'
	UNION
	SELECT '/some3.png', 4, 4, N'YouTubeImage3'
	UNION
	SELECT '/some1.png', 4, 4, N'YouTubeText1'
	UNION	
	SELECT '/some2.png', 4, 4, N'YouTubeText2'
	UNION
	SELECT '/some3.png', 4, 4, N'YouTubeText3'
	UNION
	SELECT 'jGwOsFo8TTg', 4, 4, N'YouTubeVideo1'
	UNION	
	SELECT 'btlfoO75kfI', 4, 4, N'YouTubeVideo2'
	UNION
	SELECT 'vCsRTamxWuw', 4, 4, N'YouTubeVideo3'


END