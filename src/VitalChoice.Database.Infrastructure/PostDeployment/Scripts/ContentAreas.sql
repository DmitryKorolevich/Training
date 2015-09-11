IF OBJECT_ID(N'dbo.ContentAreas', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[ContentAreas](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](250) NOT NULL,
		[Template] [nvarchar](max) NULL,
		[StatusCode] [int] NOT NULL,
		[Created] [datetime] NOT NULL,
		[Updated] [datetime] NOT NULL,
		[IdEditedBy] [int] NULL,
	 CONSTRAINT [PK_ContentAreas] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)) 

	ALTER TABLE [dbo].[ContentAreas]  WITH CHECK ADD  CONSTRAINT [FK_ContentAreas_AspNetUsers] FOREIGN KEY([IdEditedBy])
	REFERENCES [dbo].[AspNetUsers] ([Id])

	ALTER TABLE [dbo].[ContentAreas] CHECK CONSTRAINT [FK_ContentAreas_AspNetUsers]

	ALTER TABLE [dbo].[ContentAreas]  WITH CHECK ADD  CONSTRAINT [FK_ContentAreas_RecordStatusCodes] FOREIGN KEY([StatusCode])
	REFERENCES [dbo].[RecordStatusCodes] ([StatusCode])

	ALTER TABLE [dbo].[ContentAreas] CHECK CONSTRAINT [FK_ContentAreas_RecordStatusCodes]

END

IF NOT EXISTS(SELECT [Id] FROM [dbo].[ContentAreas])
BEGIN
	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Secondary Menu', N'
					<li>
						<a href="#">New</a>
					</li>
					<li>
						<a href="#">Signature Products</a>
					</li>
					<li>
						<a href="#">Special Offers</a>
					</li>
					<li>
						<a href="#">Recipes</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">All Recipes</a>
									</li>
									<li>
										<a href="#">Recipe Videos</a>
									</li>
									<li>
										<a href="#">Seafood Cooking Tips</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<a href="#">Why Vital Choice?</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">About Us</a>
									</li>
									<li>
										<a href="#">Our Mission</a>
									</li>
									<li>
										<a href="#">Testimonials</a>
									</li>
									<li>
										<a href="#">Giving Back</a>
									</li>
									<li>
										<a href="#">News Room</a>
									</li>
									<li>
										<a href="#">Vital Green™</a>
									</li>
									<li>
										<a href="#">Sustainability</a>
									</li>
									<li>
										<a href="#">Product Purity</a>
									</li>
									<li>
										<a href="#">Randy''s Podcasts</a>
									</li>
									<li>
										<a href="#">Customer Rewards</a>
									</li>
									<li>
										<a href="#">Frequent Questions (FAQs)</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<a href="#">Health & Nutrition</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">Omega-3 Basics</a>
									</li>
									<li>
										<a href="#">Seafood Benefits</a>
									</li>
									<li>
										<a href="#">Omega-3/6 Balance</a>
									</li>
									<li>
										<a href="#">Healthy Mom & Baby</a>
									</li>
									<li>
										<a href="#">Seafood Purity & Safety</a>
									</li>
									<li>
										<a href="#">Recommended Reading</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
					<li>
						<a href="#">Catalog</a>
					</li>
					<li class="last-child">
						<a href="#">Newsletter</a>
						<div class="dropdown_1column">
							<div class="col_1">
								<ul>
									<li>
										<a href="#">Sign Up</a>
									</li>
									<li>
										<a href="#">Archives</a>
									</li>
								</ul>
							</div>
						</div>
					</li>
				', 2, GETDATE(), GETDATE())

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Home Carousel', N'<a href="#"><img src="/Assets/images/King_C3_cover_slide_v2.jpg" alt=""/></a>
		<a href="#"><img src="/Assets/images/chicken-hp-banner-v6.jpg" alt=""/></a>
		<a href="#"><img src="/Assets/images/HP_banner_bonebroth_web.jpg" alt=""/></a>
		<a href="#"><img src="/Assets/images/HP_banner_sardines_2.jpg" alt=""/></a>', 2, GETDATE(), GETDATE())

	INSERT INTO [dbo].[ContentAreas]
	([Name], [Template], [StatusCode], [Created], [Updated])
	VALUES
	(N'Home Banners', N'<div>
			<img src="/Assets/images/homebanner_sausagebaconburger_v2.jpg" usemap="#btmbanner2">
		</div>
		<map name="btmbanner2" id="btmbanner2">
			<area shape="rect" coords="23,0,219,260" href="#">
			<area shape="rect" coords="227,14,440,252" href="#">
			<area shape="rect" coords="477,14,690,252" href="#">
			<area shape="rect" coords="728,14,941,252" href="#">
		</map>
		<div>
			<a href="#" target="_blank"><img src="/Assets/images/labor-day-hp-v3.jpg"></a>
		</div>
		<div>
			<img src="/Assets/images/canned-banner-v3.jpg" usemap="#btmbanner1">
		</div>
		<map name="btmbanner1" id="btmbanner1">
			<area shape="rect" coords="0,0,233,260" href="#">
			<area shape="rect" coords="234,14,447,252" href="#">
			<area shape="rect" coords="487,14,700,252" href="#">
			<area shape="rect" coords="738,14,951,252" href="#">
		</map>', 2, GETDATE(), GETDATE())
END