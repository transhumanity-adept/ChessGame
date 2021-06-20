CREATE TABLE Players
(
	[Login] varchar(100) not null primary key,
	[Password] varchar(100) not null,
	[Rating] int not null
)

CREATE TABLE Games
(
	[LoginWhitePlayer] varchar(100) not null foreign key([LoginWhitePlayer]) references Players([Login]),
	[LoginBlackPlayer] varchar(100) not null foreign key([LoginWhitePlayer]) references Players([Login]),
	[Date] datetime not null,
	primary key([LoginWhitePlayer], [LoginBlackPlayer], [Date]),
	[SerializedData] varchar(max) not null
)