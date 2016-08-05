IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas] WHERE [Name]='Wholesale Welcome Area')
BEGIN

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Wholesale Welcome Area', N'<div class="wholesale-wrapper">
		<div class="wholesale-header">
			<img src="/files/catalog/VC30images/wholesale/welcome-to-wholesale.jpg" width="710" height="215">
		</div>
		<ul class="wholesale-slugs">
			<a href="/shop/pc/viewCategories.asp?idCategory=827">
				<li>
					<img src="/files/catalog/VC30images/wholesale/canned-slug.jpg">
					<span>Canned Seafood</span>
				</li>
			</a>
			<a href="/shop/pc/viewCategories.asp?idCategory=828">
			<li>
				<img src="/files/catalog/VC30images/wholesale/supp-slug.jpg">
				<span>Dietary Supplements</span>
			</li>
			</a>
			<a href="/shop/pc/viewCategories.asp?idCategory=829">
			<li>
				<img src="/files/catalog/VC30images/wholesale/organics-slug.jpg">
				<span>Organic Foods</span>
			</li>
			</a>
		</ul>
		<div class="clear"></div>
		<div class="wholesale-message">
			<p>
				<b>To our valued wholesale customers,</b> we offer a wide variety of premium canned and pouched seafood, marine supplements, and a limited selection of organic foods at greatly reduced prices. In addition, we extend a special 15% discount on large-quantity orders of our regular frozen seafood, grass-fed beef and other products in conjunction with your wholesale purchase. Just look for the discount offers in red type as you browse our website through this wholesale portal.
			</p>
			<p>
				Questions? Please feel free to call <a href="tel:8664825887">866-482-5887</a> or email <a href="mailto:wholesale@vitalchoice.com">wholesale@vitalchoice.com</a>. You may also find answers and other important information at <a href="/shop/pc/viewContent.asp?idpage=543">FAQ</a>. 
			</p>
			<p>
				Thank you for choosing Vital Choice.
			</p>
		</div>
</div>', 2, GETDATE(), GETDATE())

END

GO