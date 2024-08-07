USE [master]
GO
/****** Object:  Database [IMRepo_Synthetic]    Script Date: 7/4/2024 10:37:43 PM ******/
CREATE DATABASE [IMRepo_Synthetic]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'IMRepo_Synthetic', FILENAME = N'C:\Users\ja_os\IMRepo_Synthetic.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'IMRepo_Synthetic_log', FILENAME = N'C:\Users\ja_os\IMRepo_Synthetic_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [IMRepo_Synthetic] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [IMRepo_Synthetic].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [IMRepo_Synthetic] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET ARITHABORT OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [IMRepo_Synthetic] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [IMRepo_Synthetic] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET  DISABLE_BROKER 
GO
ALTER DATABASE [IMRepo_Synthetic] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [IMRepo_Synthetic] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [IMRepo_Synthetic] SET  MULTI_USER 
GO
ALTER DATABASE [IMRepo_Synthetic] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [IMRepo_Synthetic] SET DB_CHAINING OFF 
GO
ALTER DATABASE [IMRepo_Synthetic] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [IMRepo_Synthetic] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [IMRepo_Synthetic] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [IMRepo_Synthetic] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [IMRepo_Synthetic] SET QUERY_STORE = OFF
GO
USE [IMRepo_Synthetic]
GO
/****** Object:  Table [dbo].[ProjectFunding]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectFunding](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Project] [int] NOT NULL,
	[Type] [int] NULL,
	[Source] [int] NOT NULL,
	[Value] [float] NOT NULL,
 CONSTRAINT [PK_ProjectFunding] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[project_fundingTotal]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[project_fundingTotal]
(   
    @projectId INT = null,
    @projectFundingId INT = null
)
RETURNS table
AS RETURN
select ProjectFunding.Project as id, 
	sum([ProjectFunding].[Value]) AS [programmed]
from ProjectFunding
where (@projectId is null or ProjectFunding.Project = @projectId)
	and (@projectFundingId is null or ProjectFunding.Id <> @projectFundingId)
group by ProjectFunding.Project
GO
/****** Object:  Table [dbo].[PaymentStage]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentStage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](30) NOT NULL,
	[SortOrder] [int] NULL,
 CONSTRAINT [PK_PaymentStage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Project]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Project](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Code] [nvarchar](20) NULL,
	[Sector] [int] NULL,
	[Subsector] [int] NULL,
	[Stage] [int] NOT NULL,
	[Office] [int] NULL,
	[Description] [nvarchar](max) NULL,
	[Objectives] [nvarchar](max) NULL,
	[PlannedCost] [float] NULL,
	[Location] [nvarchar](max) NULL,
	[ActualEndDate] [datetime2](7) NULL,
	[ActualStartDate] [datetime2](7) NULL,
	[PlannedDuration] [int] NULL,
	[PlannedStartDate] [datetime2](7) NULL,
	[Sdg] [int] NULL,
	[ExecutingAgency] [int] NULL,
 CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Project] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Cost] [float] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Objective] [nvarchar](max) NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Product] [int] NOT NULL,
	[Code] [nvarchar](15) NULL,
	[FundingSource] [int] NOT NULL,
	[ReportedMonth] [datetime2](7) NULL,
	[PaymentAmount] [float] NULL,
	[PhysicalAdvance] [real] NULL,
	[Stage] [int] NOT NULL,
	[DateDelivery] [datetime2](7) NOT NULL,
	[DateApproved] [datetime2](7) NULL,
	[DatePayed] [datetime2](7) NULL,
	[AttachmentAdvance] [nvarchar](max) NULL,
	[AttachmentPayment] [nvarchar](max) NULL,
 CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PaymentAttachment]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Payment] [int] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[File] [nvarchar](max) NOT NULL,
	[DateAttached] [datetime2](7) NULL,
 CONSTRAINT [PK_PaymentAttachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[report_Payments]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE View [dbo].[report_Payments] as

select 
[Project].Code as ProjectCode
,[Project].[Name] as ProjectName
,Payment.Code as paymentCode
,PaymentStage.Title as stageName
,payment.ReportedMonth as reportedMonth
,Payment.[PaymentAmount] as paymentValue
,Payment.PhysicalAdvance as PhysicalAdvance
,Payment.DateDelivery as DateDelivery
,Payment.DateApproved as DateApproved
,Payment.DatePayed as DatePayed
,case when Payment.AttachmentAdvance is not null then 'Sí' else 'No' end as attachedMedicion
,case when Payment.AttachmentPayment is not null then 'Sí' else 'No' end  as attachedOrden
,case when attachmentsInfo.qty > 0 then cast(attachmentsInfo.qty as varchar(10)) else 'No' end  as otherAttachments

,CASE 
    WHEN Payment.Stage < 2 THEN 
        DATEDIFF(DAY, Payment.DateDelivery, CAST(GETDATE() AS Date))
    WHEN Payment.Stage = 2 THEN 
        DATEDIFF(DAY, Payment.DateDelivery, Payment.DateApproved)
    ELSE 
        DATEDIFF(DAY, Payment.DateDelivery, Payment.DatePayed) 
END AS PaymentDelay
from [Project]
left join Product on Product.Project = Project.Id
left join Payment on Payment.[Product] = [Product].Id
left join PaymentStage on PaymentStage.Id = Payment.[Stage]
left join	
	(select PaymentAttachment.Payment as paymentId, COUNT(PaymentAttachment.id) as qty
	from PaymentAttachment 
	group by PaymentAttachment.Payment) 
	as attachmentsInfo on attachmentsInfo.paymentId = [Payment].Id
where Payment.id is not null
GO
/****** Object:  Table [dbo].[TaskStage]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskStage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[Order] [int] NULL,
 CONSTRAINT [PK_TaskStage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Addition]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Addition](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Product] [int] NOT NULL,
	[Code] [nvarchar](15) NULL,
	[Value] [float] NOT NULL,
	[Stage] [int] NOT NULL,
	[DateDelivery] [datetime2](7) NOT NULL,
	[DateApproved] [datetime2](7) NULL,
	[Notes] [nvarchar](max) NULL,
	[Attachment] [nvarchar](max) NULL,
 CONSTRAINT [PK_Addition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdditionAttachment]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdditionAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Addition] [int] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[FileName] [nvarchar](max) NOT NULL,
	[DateAttached] [datetime2](7) NULL,
 CONSTRAINT [PK_AdditionAttachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[report_Additions]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO









CREATE View [dbo].[report_Additions] as
select 
[Project].Code as ProjectCode
,[Project].[Name] as ProjectName
,Addition.Code as additionCode
,TaskStage.[Name] as stageName
,Addition.[Value] as additionValue
,Addition.DateDelivery as DateDelivery
,Addition.DateApproved as DateApproved
,case when Addition.Attachment is not null then 'Sí' else 'No' end as attached
,case when attachmentsInfo.qty > 0 then cast(attachmentsInfo.qty as varchar(10)) else 'No' end  as otherAttachments
,CASE WHEN Addition.Stage < 2 THEN 
		DATEDIFF(DAY,Addition.DateDelivery,CAST( GETDATE() AS Date ))
	ELSE 
		DATEDIFF(DAY,Addition.DateDelivery,Addition.DateApproved) 
	END AS AdditionDelay
from [Project]
left join Product on Product.[Project] = [Project].Id
left join Addition on Addition.[Product] = [Product].Id
left join TaskStage on TaskStage.Id = Addition.[Stage]
left join	
	(select AdditionAttachment.Addition as additionId, COUNT(AdditionAttachment.id) as qty
	from AdditionAttachment 
	group by AdditionAttachment.Addition) 
	as attachmentsInfo on attachmentsInfo.additionId = [Addition].Id
where Addition.Id is not null

GO
/****** Object:  Table [dbo].[Extension]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Extension](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Project] [int] NOT NULL,
	[Code] [nvarchar](15) NULL,
	[Days] [int] NOT NULL,
	[Motive] [nvarchar](250) NULL,
	[DateDelivery] [datetime2](7) NOT NULL,
	[DateApproved] [datetime2](7) NULL,
	[Stage] [int] NOT NULL,
	[Attachment] [nvarchar](max) NULL,
 CONSTRAINT [PK_Extension] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExtensionAttachment]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExtensionAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Extension] [int] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[FileName] [nvarchar](max) NOT NULL,
	[DateAttached] [datetime2](7) NULL,
 CONSTRAINT [PK_ExtensionAttachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[report_Extensions]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[report_Extensions] AS

select 
[Project].Code as ProjectCode
,[Project].[Name] as ProjectName
,Extension.Code as extensionCode
,TaskStage.[Name] as stageName
,Extension.[Days] as [days]
,Extension.DateDelivery as DateDelivery
,Extension.DateApproved as DateApproved
,case when Extension.Attachment is not null then 'Sí' else 'No' end as attached
,case when attachmentsInfo.qty > 0 then cast(attachmentsInfo.qty as varchar(10)) else 'No' end  as otherAttachments
,Extension.motive
,CASE WHEN Extension.Stage < 2 THEN 
		DATEDIFF(DAY,Extension.DateDelivery,CAST( GETDATE() AS Date ))
	ELSE 
		DATEDIFF(DAY,Extension.DateDelivery,Extension.DateApproved) 
	END AS ExtensionDelay
from [Project]
left join Extension on Extension.[Project] = [Project].Id
left join TaskStage on TaskStage.Id = Extension.[Stage]
left join	
	(select ExtensionAttachment.Extension as extensionId, COUNT(ExtensionAttachment.id) as qty
	from ExtensionAttachment 
	group by ExtensionAttachment.Extension) 
	as attachmentsInfo on attachmentsInfo.extensionId = [Extension].Id
where Extension.id is not null
GO
/****** Object:  Table [dbo].[Office]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Office](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Office] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectStage]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectStage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Order] [int] NOT NULL,
 CONSTRAINT [PK_ProjectStage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sector]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sector](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Sector] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subsector]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subsector](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Sector] [int] NOT NULL,
	[Name] [nvarchar](100) NULL,
 CONSTRAINT [PK_Subsector] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[report_ProjectBasicInfo]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[report_ProjectBasicInfo] AS
select project.Id as id_project
,project.Code as code_project
,project.[Name] as name_project
,Office.[Name] as name_office
,projectStage.[name] as name_projectStage
,sector.[name] as name_sector
,Subsector.[name] as name_subsector
from project
left join Office on Office.id = project.Office
left join projectStage on projectStage.id = project.stage
left join sector on sector.id = project.sector
left join Subsector on Subsector.id = project.Subsector
GO
/****** Object:  View [dbo].[report_ProjectPerformance]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE view [dbo].[report_ProjectPerformance] as

With 
ProjectPayments as (
SELECT 
Product.[Project] as ProjectId
, sum(payment.[PaymentAmount]) AS [value]
, sum(payment.[PhysicalAdvance]) AS [performed]
from Payment
left join Product on Product.id = Payment.Product
where Payment.Stage in (2,3) --2: aprobado, 3: pagado
GROUP BY Product.[Project]
)

,ProjectAdditions as (
SELECT 
Product.[Project] as ProjectId
, sum(addition.[Value]) as [value]
from Addition
left join Product on Product.Id = Addition.Product
where stage = 2 -- 2:Aprobada
group by Product.[Project]
)

,ProjectExtensions as (
SELECT 
Extension.[Project] as ProjectId
, sum(Extension.[Days]) as [extensionDays]
from Extension
where stage = 2 -- 2:Aprobada
group by Extension.[Project]
)

,ProjectProducts as (
SELECT 
Product.[Project] as ProjectId
, sum(Product.[Cost]) as [Cost]
from Product
group by Product.[Project]
)


select 
[Project].id as [projectId]
,[Project].Code [projectCode]
,[Project].[Name] [projectName]
-- finance
,project.PlannedCost as plannedCost
,nullif(
	isnull(ProjectProducts.Cost,0) 
	+ isnull(ProjectAdditions.[value],0) 
,0) as [ProgrammedCost]
,ProjectPayments.[value] as paymentsTotal

,case when 
1 < (ProjectPayments.[value] /
 nullif(
	isnull(ProjectProducts.Cost,0) 
		+ isnull(ProjectAdditions.[value],0)
 ,0))
 then 1
 else
ProjectPayments.[value] /
 nullif(
	isnull(ProjectProducts.Cost,0) 
	+ isnull(ProjectAdditions.[value],0) 
 ,0) * 100
 end
 as [financialRate]
,ProjectPayments.[performed] as physicalRate
--dates
,[Project].ActualStartDate as actualStartDate
,[Project].ActualEndDate as actualEndDate
,nullif(isnull([Project].PlannedDuration,0) + isnull(ProjectExtensions.extensionDays,0),null) as extensionDays
,DATEADD(DAY,isnull([Project].PlannedDuration,0) + isnull(ProjectExtensions.[extensionDays],0),[Project].ActualStartDate) as ProgrammedEndDate
,case when ([Project].ActualEndDate is null) then
DATEDIFF(DAY,[Project].ActualStartDate, GETDATE())
else
DATEDIFF(DAY,[Project].ActualStartDate, [Project].ActualEndDate)
end
as elapsedTime
,
	case when ([Project].ActualEndDate is null) then 
			cast(DATEDIFF(DAY,[Project].ActualStartDate, GETDATE()) as float) / 
			nullif(isnull([Project].PlannedDuration,0) + isnull(ProjectExtensions.[extensionDays],0),0) * 100
		else
		100
	end 
 as timeRate
from [Project]
left join ProjectPayments on [Project].id = ProjectPayments.ProjectId
left join ProjectAdditions on [Project].id = ProjectAdditions.ProjectId
left join ProjectExtensions on [Project].id = ProjectExtensions.ProjectId
left join ProjectProducts on [Project].id = ProjectProducts.ProjectId

GO
/****** Object:  View [dbo].[dashboard_info]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[dashboard_info] AS






WITH 
PaymentInfo AS (
    SELECT
        [Product].[Project] as projectId
		,SUM([Payment].[PaymentAmount]) as [TotalValue]
        ,SUM([Payment].[PhysicalAdvance]) AS PhysicalAdvance
		,SUM(CASE WHEN [Payment].[Stage] <= 2  THEN 1 ELSE 0 END) AS PendingPaymentsCount
		,SUM(CASE WHEN [Payment].[Stage] <= 2 THEN [Payment].[PaymentAmount] ELSE 0 END) AS PendingPaymentsValue
    FROM
        [Payment] 
		left join [Product] on [Product].id = [Payment].[Product]
    GROUP BY
        [Product].[Project]
	)
,AdditionInfo AS (
    SELECT
        [Product].[Project] as projectId
		,SUM([Addition].[Value]) as [Value]
		,SUM(CASE WHEN [Addition].[Stage] < 3  THEN 1 ELSE 0 END) AS PendingAdditionsCount
		,SUM(CASE WHEN [Addition].[Stage] < 3  THEN [Addition].[Value] ELSE 0 END) AS PendingAdditionsValue
    FROM
        [Addition] 
		left join [Product] on [Product].id = [Addition].[Product]
    GROUP BY
        [Product].[Project]
)
,ExtensionInfo AS (
    SELECT
        [Extension].[Project]
		,SUM(CASE WHEN [Extension].[Stage] < 3  THEN 1 ELSE 0 END) AS PendingExtensionsCount
    FROM
        [Extension] 
    GROUP BY
        [Extension].[Project]
)

SELECT
    [Project].[Office],
    [Project].[Sector],
    [Project].[SubSector],
    SUM(CASE WHEN [Project].[ActualStartDate] is null THEN 1 ELSE 0 END) AS NotStartedProjectsCount,
    SUM(CASE WHEN [Project].[ActualStartDate] is null THEN
		ISNULL([Project].[PlannedCost], 0)
		ELSE 0 END) / 1000000 AS NotStartedProjectsValue,

    SUM(CASE WHEN [Project].[ActualStartDate] is not null AND [Project].[ActualEndDate] is null THEN 1 ELSE 0 END) AS OngoingProjectsCount,
    SUM(CASE WHEN [Project].[ActualStartDate] is not null AND [Project].[ActualEndDate] is null THEN
        ISNULL([Project].[PlannedCost], 0) + ISNULL([AdditionInfo].[Value], 0) 
    ELSE null END) / 1000000 AS OngoingProjectsCost,
    SUM(CASE WHEN [Project].[ActualEndDate] is not null THEN 1 ELSE 0 END) AS FinishedProjectsCount,
    SUM(CASE WHEN [Project].[ActualEndDate] is not null THEN
        ISNULL([Project].[PlannedCost], 0) + ISNULL([AdditionInfo].[Value], 0)
    ELSE null END) / 1000000 AS FinishedProjectsCost,
    SUM(CASE WHEN [Project].[ActualStartDate] is null  THEN 1 ELSE 0 END) AS ToStartProjectsCount,
    SUM(
        ISNULL([Project].[PlannedCost], 0) + ISNULL([AdditionInfo].[Value], 0) 
    ) / 1000000 AS TotalCostOfAllProducts,
    SUM(
        ISNULL([PaymentInfo].[TotalValue], 0) 
    ) / 1000000 AS TotalPayedValue,
    SUM(
        ISNULL([Project].[PlannedCost], 0) + ISNULL([AdditionInfo].[Value], 0) 
        - ISNULL([PaymentInfo].[TotalValue], 0)
		) / 1000000 AS TotalRemainingValue,

	SUM([PaymentInfo].PendingPaymentsCount) as PendingPaymentsCount,
	SUM([PaymentInfo].PendingPaymentsValue) as PendingPaymentsValue,
	SUM([AdditionInfo].PendingAdditionsCount) as PendingAdditionsCount,
	SUM([AdditionInfo].PendingAdditionsValue) as PendingAdditionsValue,
	SUM([ExtensionInfo].PendingExtensionsCount) as PendingExtensionsCount,

    SUM(CASE WHEN YEAR([Project].[ActualEndDate]) = YEAR(GETDATE()) THEN 1 ELSE 0 END) AS ProjectsEndedCurrentYear,
    SUM(CASE WHEN YEAR([Project].[ActualEndDate]) = YEAR(GETDATE()) - 1 THEN 1 ELSE 0 END) AS ProjectsEndedLastYear,
    SUM(CASE WHEN YEAR([Project].[ActualEndDate]) = YEAR(GETDATE()) - 2 THEN 1 ELSE 0 END) AS ProjectsEndedTwoYearsAgo,
    SUM(CASE WHEN YEAR([Project].[ActualEndDate]) = YEAR(GETDATE()) - 3 THEN 1 ELSE 0 END) AS ProjectsEndedThreeYearsAgo,

    SUM(CASE WHEN ([Project].[ActualStartDate] is not null AND [Project].[ActualEndDate] is null) AND 
				([PaymentInfo].[PhysicalAdvance] IS NULL OR [PaymentInfo].[PhysicalAdvance] = 0) THEN 1 ELSE 0 END) AS ProjectsNoAdvance,
    SUM(CASE WHEN ([Project].[ActualStartDate] is not null AND [Project].[ActualEndDate] is null) AND 
				([PaymentInfo].[PhysicalAdvance] > 0 AND [PaymentInfo].[PhysicalAdvance] <= 25) THEN 1 ELSE 0 END) AS ProjectsAdvance0to25,
    SUM(CASE WHEN ([Project].[ActualStartDate] is not null AND [Project].[ActualEndDate] is null) AND 
				([PaymentInfo].[PhysicalAdvance] > 25 AND [PaymentInfo].[PhysicalAdvance] <= 50) THEN 1 ELSE 0 END) AS ProjectsAdvance25to50,
    SUM(CASE WHEN ([Project].[ActualStartDate] is not null AND [Project].[ActualEndDate] is null) AND 
				([PaymentInfo].[PhysicalAdvance] > 50 AND [PaymentInfo].[PhysicalAdvance] <= 75) THEN 1 ELSE 0 END) AS ProjectsAdvance50to75,
    SUM(CASE WHEN ([Project].[ActualStartDate] is not null AND [Project].[ActualEndDate] is null) AND 
				([PaymentInfo].[PhysicalAdvance] > 75 AND [PaymentInfo].[PhysicalAdvance] <= 100) THEN 1 ELSE 0 END) AS ProjectsAdvance75to100,
    
	SUM(CASE WHEN [Project].[ActualStartDate] is null THEN 1 ELSE 0 END) AS ProjectsStageNotStarted,
    SUM(CASE WHEN [Project].[ActualStartDate] is not null and [Project].[ActualEndDate] is null  THEN 1 ELSE 0 END) AS ProjectsStageOngoing,
    SUM(CASE WHEN [Project].[ActualEndDate] is not null  THEN 1 ELSE 0 END) AS ProjectsStageEnded,

(SELECT 
(
'{"type": "FeatureCollection", "features":[' 
+
replace(replace(STRING_AGG(JSON_QUERY(location, '$.features'), ','),'[{','{'),'}]','}')
+
']}'
) as locations

FROM dbo.Project
WHERE
-- features not empty
location <> '{"type":"FeatureCollection","features":[]}'
) as LocationInfo


FROM [Project]
LEFT OUTER JOIN PaymentInfo on [Project].[Id] = PaymentInfo.[projectId]
LEFT JOIN AdditionInfo on [Project].[Id] = AdditionInfo.[projectId]
LEFT JOIN ExtensionInfo on [Project].[Id] = ExtensionInfo.[Project]
GROUP BY
    [Project].[Office],
    [Project].[Sector],
    [Project].[SubSector];







GO
/****** Object:  View [dbo].[dashboard_ProjectsPerSector]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[dashboard_ProjectsPerSector] AS
select Sector.Id as itemId
,Sector.[name] as itemName
,COUNT(project.id) as itemCount
from Sector 
left join project on sector.Id = project.Sector
group by sector.Id,Sector.[Name]
GO
/****** Object:  View [dbo].[addition_Totals]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[addition_Totals]
As

With 

totalAdditions as (
SELECT 
Product.[Project] as projectId
, count(Addition.id) as number
, sum(addition.[Value]) as [value]
from Addition
left join Product on Product.id = Addition.Product
group by Product.[Project]
)

,requestedAdditions as (
SELECT 
Product.[Project] as projectId
, count(Addition.id) as number
, sum(addition.[Value]) as [value]
from Addition
left join Product on Product.id = Addition.Product
where stage in (1) -- 1:Presentada
group by Product.[Project]
)

,approvedAdditions as (
SELECT 
Product.[Project] as projectId
, count(Addition.id) as number
, sum(addition.[Value]) as [value]
from Addition
left join Product on Product.id = Addition.Product
where stage in (2) -- 2:Aprobada
group by Product.[Project]
)



-- MAIN SELECT
select 
[Project].Id 
,totalAdditions.number as totalQty
,totalAdditions.value as totalValue
,requestedAdditions.number as requestedQty
,requestedAdditions.[value] as requestedValue
,approvedAdditions.number as approvedQty
,approvedAdditions.[value] as approvedValue

from [Project]
left join totalAdditions on [Project].id = totalAdditions.projectId
left join requestedAdditions on [Project].id = requestedAdditions.projectId
left join approvedAdditions on [Project].id = approvedAdditions.projectId


GO
/****** Object:  View [dbo].[extension_Totals]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[extension_Totals]
AS


With 

totalExtensions as (
SELECT 
Extension.[Project] as projectId
, count(Extension.id) as number
, sum(Extension.[Days]) as [value]
from Extension
group by Extension.[Project]
)

,requestedExtensions as (
SELECT 
Extension.[Project] as projectId
, count(Extension.id) as number
, sum(Extension.[Days]) as [value]
from Extension
where stage in (1) -- 1:Presentada
group by Extension.[Project]
)

,approvedExtensions as (
SELECT 
Extension.[Project] as projectId
, count(Extension.id) as number
, sum(Extension.[Days]) as [value]
from Extension
where stage in (2) -- 2:Aprobada
group by Extension.[Project]
)

-- MAIN SELECT
select 
[Project].Id 
,totalExtensions.number as totalQty
,totalExtensions.value as totalValue
,requestedExtensions.number as requestedQty
,requestedExtensions.[value] as requestedValue
,approvedExtensions.number as approvedQty
,approvedExtensions.[value] as approvedValue

from [Project]
left join totalExtensions on [Project].id = totalExtensions.projectId
left join requestedExtensions on [Project].id = requestedExtensions.projectId
left join approvedExtensions on [Project].id = approvedExtensions.projectId




GO
/****** Object:  View [dbo].[payment_Totals]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[payment_Totals]
AS

With 

totalPayments as (
SELECT 
[Product].[Project] as projectId
, count(payment.id) as number
, sum(payment.[PaymentAmount]) AS [Total]
from Payment
left join Product on Product.id = Payment.Product
GROUP BY [Product].[Project]
)

,requestedPayments as (
SELECT 
[Product].[Project] as projectId
, count(payment.id) as number
, sum(payment.[PaymentAmount]) AS [Total]
from Payment
left join Product on Product.id = Payment.Product
where stage = 1
GROUP BY [Product].[Project]
)

,approvedPayments as (
SELECT 
[Product].[Project] as projectId
, count(payment.id) as number
, sum(payment.[PaymentAmount]) AS [Total]
from Payment
left join Product on Product.id = Payment.Product
where Payment.DateApproved is not null
GROUP BY [Product].[Project]
)


,productAdditions as (
SELECT 
[Product].[Project] as projectId
, sum(addition.[Value]) as [value]
from Addition
left join Product on Product.id = Addition.Product
where stage in (2) -- 2:Aprobada
GROUP BY [Product].[Project]
)

,productsCost as (
SELECT 
[Product].[Project] as projectId
, sum([Product].[Cost]) as [value]
from [Product]
GROUP BY [Product].[Project]
)


-- MAIN SELECT
select 
[Project].Id
,totalPayments.number as totalQty
,totalPayments.Total as totalValue
,requestedPayments.number as requestedQty
,requestedPayments.[Total] as requestedValue
,approvedPayments.number as approvedQty
,approvedPayments.[Total] as approvedValue
,nullif(
	isnull([productsCost].[value],0) 
	+ isnull(productAdditions.[value],0) 
	- totalPayments.[Total]
,0) as [available]

from [Project]
left join totalPayments on     [Project].id = totalPayments.projectId
left join requestedPayments on [Project].id = requestedPayments.projectId
left join approvedPayments on  [Project].id = approvedPayments.projectId
left join productAdditions on [Project].id = productAdditions.projectId
left join productsCost on [Project].id = productsCost.projectId



GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Agency]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Agency](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Acronym] [nvarchar](50) NULL,
	[OfficialID] [nvarchar](25) NULL,
 CONSTRAINT [PK_Agency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[City]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[City](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Province] [int] NOT NULL,
	[Name] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FundingAgency]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundingAgency](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Acronym] [nvarchar](20) NULL,
 CONSTRAINT [PK_FundingAgency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FundingType]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundingType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_FundingType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectAttachment]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Project] [int] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[FileName] [nvarchar](max) NULL,
	[DateAttached] [datetime2](7) NULL,
 CONSTRAINT [PK_ProjectAttachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectImage]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectImage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Project] [int] NOT NULL,
	[File] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[ImageDate] [datetime2](7) NULL,
	[UploadDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ProjectImage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectType]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](25) NOT NULL,
 CONSTRAINT [PK_ProjectType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectVideo]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectVideo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Project] [int] NOT NULL,
	[Link] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[VideoDate] [datetime2](7) NULL,
	[UploadDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ProjectVideo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Province]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Province](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Province] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sdg]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sdg](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] [int] NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Sdg] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserProfile]    Script Date: 7/4/2024 10:37:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AspNetUserId] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[Name] [nvarchar](25) NOT NULL,
	[Surname] [nvarchar](25) NOT NULL,
	[Office] [int] NULL,
	[Notes] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Addition] ON 

INSERT [dbo].[Addition] ([Id], [Product], [Code], [Value], [Stage], [DateDelivery], [DateApproved], [Notes], [Attachment]) VALUES (1, 25, N'01001', 500000, 2, CAST(N'2023-10-17T00:00:00.0000000' AS DateTime2), CAST(N'2023-10-31T00:00:00.0000000' AS DateTime2), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Addition] OFF
GO
SET IDENTITY_INSERT [dbo].[Agency] ON 

INSERT [dbo].[Agency] ([Id], [Name], [Acronym], [OfficialID]) VALUES (1, N'Secretaría de Gobierno', N'SEGOB', N'GOV001')
INSERT [dbo].[Agency] ([Id], [Name], [Acronym], [OfficialID]) VALUES (2, N'Secretaría de Educación', N'SEDUC', N'EDU002')
INSERT [dbo].[Agency] ([Id], [Name], [Acronym], [OfficialID]) VALUES (3, N'Secretaría de Salud', N'SSAL', N'SAL003')
INSERT [dbo].[Agency] ([Id], [Name], [Acronym], [OfficialID]) VALUES (4, N'Secretaría de Deportes', N'SDEPO', N'DEP004')
INSERT [dbo].[Agency] ([Id], [Name], [Acronym], [OfficialID]) VALUES (5, N'Secretaría de Energía', N'SENER', N'ENE005')
INSERT [dbo].[Agency] ([Id], [Name], [Acronym], [OfficialID]) VALUES (6, N'Secretaría de Obras Públicas', N'SOP', N'OP006')
INSERT [dbo].[Agency] ([Id], [Name], [Acronym], [OfficialID]) VALUES (7, N'Secretaría de Vivienda', N'SVIV', N'VIV007')
INSERT [dbo].[Agency] ([Id], [Name], [Acronym], [OfficialID]) VALUES (8, N'Secretaría de Recursos Hídricos', N'SRH', N'RH008')
INSERT [dbo].[Agency] ([Id], [Name], [Acronym], [OfficialID]) VALUES (9, N'Secretaría de Desarrollo Económico', N'SDECO', N'DECO009')
INSERT [dbo].[Agency] ([Id], [Name], [Acronym], [OfficialID]) VALUES (10, N'Secretaría de Servicios Públicos', N'SSP', N'SP010')
SET IDENTITY_INSERT [dbo].[Agency] OFF
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'1', N'Administrator', NULL, NULL)
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'2', N'Direccion', NULL, NULL)
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'3', N'Operacion', NULL, NULL)
INSERT [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'4', N'Consulta', NULL, NULL)
GO
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'481437b0-2c9c-4267-a91e-a7ffe6290224', N'1')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'824c4b6f-473f-4c8e-849f-1667407dc10c', N'3')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'8a7da9ee-f64a-45af-b06e-ccd0196fa9c6', N'2')
GO
INSERT [dbo].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'481437b0-2c9c-4267-a91e-a7ffe6290224', N'admin@mail.com', N'ADMIN@MAIL.COM', N'admin@mail.com', N'ADMIN@MAIL.COM', 1, N'AQAAAAEAACcQAAAAEOrBy6mjghWIZ0SA28cnwF0w/8ZpL5VpQmuMUwda14OtKyzOTEgOrwWjdEQdRLXHZw==', N'WBEYUVPEF56QFTVVHNHZNGV7DTBKFO6J', N'31daf3f8-bc31-46c9-8711-354cafa95846', NULL, 0, 0, NULL, 1, 0)
INSERT [dbo].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'824c4b6f-473f-4c8e-849f-1667407dc10c', N'operacion@mail.com', N'OPERACION@MAIL.COM', N'operacion@mail.com', N'OPERACION@MAIL.COM', 1, N'AQAAAAEAACcQAAAAEKC5ikk+l0yyQPLdfivATzmqOqEzB+gGbptEloq9Zk13BrPfmPewGh/hfssXY9pVtw==', N'FBW3U73WG3HJDZLRFYYONAPAJ544S6JG', N'3cd04ed9-1271-4a11-ab4c-0aca4b908a98', NULL, 0, 0, NULL, 1, 0)
INSERT [dbo].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'8a7da9ee-f64a-45af-b06e-ccd0196fa9c6', N'direccion@mail.com', N'DIRECCION@MAIL.COM', N'direccion@mail.com', N'DIRECCION@MAIL.COM', 1, N'AQAAAAEAACcQAAAAECefbuPJqtXGVxD73qDeKRNwXUBEoURw2/kRueZtcPH7y3R+23vxlqy7Vzs7Fo7kHQ==', N'53RJS3OGG4PHFOPSBGH63OP4MZKRC5ZO', N'47dbf5ea-fb60-4ce1-bcba-228cc7854adf', NULL, 0, 0, NULL, 1, 0)
INSERT [dbo].[AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'd0f4ce17-1347-4596-8688-f5088dc34762', N'consulta@mail.com', N'CONSULTA@MAIL.COM', N'consulta@mail.com', N'CONSULTA@MAIL.COM', 1, N'AQAAAAEAACcQAAAAEL9LTvnGCXP5cLGHX8WJA9yg4C7v3xxvnkV7WyuPDkw72o8LlpVVlHQTt9r+vk3e0Q==', N'KAEU4OPBMYHSRI77ZMSQR6SIYFSD73WJ', N'2e0f572e-77a3-4924-b079-b85bf11ffef2', NULL, 0, 0, NULL, 1, 0)
GO
SET IDENTITY_INSERT [dbo].[Extension] ON 

INSERT [dbo].[Extension] ([Id], [Project], [Code], [Days], [Motive], [DateDelivery], [DateApproved], [Stage], [Attachment]) VALUES (1, 7, N'01001', 90, NULL, CAST(N'2023-11-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-17T00:00:00.0000000' AS DateTime2), 2, NULL)
SET IDENTITY_INSERT [dbo].[Extension] OFF
GO
SET IDENTITY_INSERT [dbo].[FundingAgency] ON 

INSERT [dbo].[FundingAgency] ([Id], [Name], [Acronym]) VALUES (1, N'Gobierno Nacional', N'GN')
INSERT [dbo].[FundingAgency] ([Id], [Name], [Acronym]) VALUES (2, N'Banco de Desarrollo', N'BD')
INSERT [dbo].[FundingAgency] ([Id], [Name], [Acronym]) VALUES (3, N'Banco Interamericano de Desarrollo', N'BID')
INSERT [dbo].[FundingAgency] ([Id], [Name], [Acronym]) VALUES (4, N'Banco Mundial', N'BM')
INSERT [dbo].[FundingAgency] ([Id], [Name], [Acronym]) VALUES (5, N'Organización de Estados Americanos', N'OEA')
INSERT [dbo].[FundingAgency] ([Id], [Name], [Acronym]) VALUES (6, N'Fondo Monetario Internacional', N'FMI')
INSERT [dbo].[FundingAgency] ([Id], [Name], [Acronym]) VALUES (7, N'Agencia de Cooperación Internacional de Japón', N'JICA')
INSERT [dbo].[FundingAgency] ([Id], [Name], [Acronym]) VALUES (8, N'Banco Europeo de Inversiones', N'BEI')
INSERT [dbo].[FundingAgency] ([Id], [Name], [Acronym]) VALUES (9, N'Banco de Desarrollo de América Latina', N'CAF')
INSERT [dbo].[FundingAgency] ([Id], [Name], [Acronym]) VALUES (10, N'Gobierno de Estados Unidos', N'USG')
SET IDENTITY_INSERT [dbo].[FundingAgency] OFF
GO
SET IDENTITY_INSERT [dbo].[FundingType] ON 

INSERT [dbo].[FundingType] ([Id], [Name]) VALUES (1, N'Gobierno')
INSERT [dbo].[FundingType] ([Id], [Name]) VALUES (2, N'Crédito')
INSERT [dbo].[FundingType] ([Id], [Name]) VALUES (3, N'Donación')
INSERT [dbo].[FundingType] ([Id], [Name]) VALUES (4, N'APP')
SET IDENTITY_INSERT [dbo].[FundingType] OFF
GO
SET IDENTITY_INSERT [dbo].[Office] ON 

INSERT [dbo].[Office] ([Id], [Name]) VALUES (1, N'Secretaría de Gobierno')
INSERT [dbo].[Office] ([Id], [Name]) VALUES (2, N'Secretaría de Educación')
INSERT [dbo].[Office] ([Id], [Name]) VALUES (3, N'Secretaría de Salud')
INSERT [dbo].[Office] ([Id], [Name]) VALUES (4, N'Secretaría de Obras Públicas')
INSERT [dbo].[Office] ([Id], [Name]) VALUES (5, N'Secretaría de Desarrollo Social')
INSERT [dbo].[Office] ([Id], [Name]) VALUES (6, N'Secretaría de Cultura')
INSERT [dbo].[Office] ([Id], [Name]) VALUES (7, N'Secretaría de Medio Ambiente')
INSERT [dbo].[Office] ([Id], [Name]) VALUES (8, N'Secretaría de Economía')
INSERT [dbo].[Office] ([Id], [Name]) VALUES (9, N'Secretaría de Seguridad Pública')
INSERT [dbo].[Office] ([Id], [Name]) VALUES (10, N'Secretaría de Finanzas')
SET IDENTITY_INSERT [dbo].[Office] OFF
GO
SET IDENTITY_INSERT [dbo].[Payment] ON 

INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2672, 15, N'1000', 3, CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), 57792.38, 4.43, 2, CAST(N'2023-06-11T00:00:00.0000000' AS DateTime2), CAST(N'2023-06-12T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2673, 15, N'1001', 4, CAST(N'2023-08-01T00:00:00.0000000' AS DateTime2), 57415.14, 4.55, 1, CAST(N'2023-06-08T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2674, 15, N'1002', 4, CAST(N'2023-09-01T00:00:00.0000000' AS DateTime2), 84792.48, 5.29, 1, CAST(N'2023-06-09T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2675, 16, N'1003', 4, CAST(N'2023-10-01T00:00:00.0000000' AS DateTime2), 331870.89, 19.83, 1, CAST(N'2023-06-10T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2676, 16, N'1004', 4, CAST(N'2023-11-01T00:00:00.0000000' AS DateTime2), 668129.11, 51.07, 1, CAST(N'2023-06-02T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2677, 19, N'1005', 7, CAST(N'2023-10-01T00:00:00.0000000' AS DateTime2), 500000, 49.22, 1, CAST(N'2023-09-03T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2678, 20, N'1006', 8, CAST(N'2023-11-01T00:00:00.0000000' AS DateTime2), 128350.74, 15.74, 1, CAST(N'2023-09-09T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2679, 20, N'1007', 8, CAST(N'2023-12-01T00:00:00.0000000' AS DateTime2), 171649.26, 20.07, 2, CAST(N'2023-09-02T00:00:00.0000000' AS DateTime2), CAST(N'2023-09-03T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2680, 23, N'1008', 12, CAST(N'2023-12-01T00:00:00.0000000' AS DateTime2), 15063.05, 1.37, 1, CAST(N'2023-11-08T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2681, 23, N'1009', 11, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2), 230885.68, 16.57, 2, CAST(N'2023-11-06T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-08T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2682, 23, N'1010', 11, CAST(N'2024-02-01T00:00:00.0000000' AS DateTime2), 254051.27, 17.96, 1, CAST(N'2023-11-02T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2683, 24, N'1011', 12, CAST(N'2024-03-01T00:00:00.0000000' AS DateTime2), 73631.96, 6.39, 2, CAST(N'2023-11-08T00:00:00.0000000' AS DateTime2), CAST(N'2023-11-10T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2684, 24, N'1012', 12, CAST(N'2024-04-01T00:00:00.0000000' AS DateTime2), 426368.04, 35.1, 1, CAST(N'2023-11-06T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2685, 25, N'1013', 14, CAST(N'2023-11-01T00:00:00.0000000' AS DateTime2), 255117.9, 12.77, 1, CAST(N'2023-10-05T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2686, 25, N'1014', 13, CAST(N'2023-12-01T00:00:00.0000000' AS DateTime2), 744882.1, 37.08, 1, CAST(N'2023-10-04T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2687, 26, N'1015', 13, CAST(N'2024-01-01T00:00:00.0000000' AS DateTime2), 19155.45, 1.05, 1, CAST(N'2023-10-09T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2688, 26, N'1016', 13, CAST(N'2024-02-01T00:00:00.0000000' AS DateTime2), 52352.26, 3.2, 1, CAST(N'2023-10-11T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2689, 26, N'1017', 13, CAST(N'2024-03-01T00:00:00.0000000' AS DateTime2), 428492.29, 24.51, 2, CAST(N'2023-10-11T00:00:00.0000000' AS DateTime2), CAST(N'2023-10-13T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2690, 29, N'1018', 17, CAST(N'2023-09-01T00:00:00.0000000' AS DateTime2), 300000, 43.13, 2, CAST(N'2023-08-09T00:00:00.0000000' AS DateTime2), CAST(N'2023-08-11T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2691, 30, N'1019', 18, CAST(N'2023-10-01T00:00:00.0000000' AS DateTime2), 92175.08, 14.44, 2, CAST(N'2023-08-06T00:00:00.0000000' AS DateTime2), CAST(N'2023-08-07T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2692, 30, N'1020', 18, CAST(N'2023-11-01T00:00:00.0000000' AS DateTime2), 107824.92, 16.86, 1, CAST(N'2023-08-10T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2693, 31, N'1021', 19, CAST(N'2023-10-01T00:00:00.0000000' AS DateTime2), 87679.18, 19.51, 1, CAST(N'2023-09-06T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2694, 31, N'1022', 20, CAST(N'2023-11-01T00:00:00.0000000' AS DateTime2), 112320.82, 23.09, 1, CAST(N'2023-09-03T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2695, 32, N'1023', 113, CAST(N'2023-02-20T00:00:00.0000000' AS DateTime2), 35938.21, 2.24, 1, CAST(N'2023-01-24T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2696, 32, N'1024', 113, CAST(N'2023-03-20T00:00:00.0000000' AS DateTime2), 42469.92, 2.73, 2, CAST(N'2023-01-21T00:00:00.0000000' AS DateTime2), CAST(N'2023-01-23T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2697, 32, N'1025', 113, CAST(N'2023-04-20T00:00:00.0000000' AS DateTime2), 130310.67, 8, 3, CAST(N'2023-01-25T00:00:00.0000000' AS DateTime2), CAST(N'2023-01-26T00:00:00.0000000' AS DateTime2), CAST(N'2023-01-27T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2698, 32, N'1026', 113, CAST(N'2023-05-20T00:00:00.0000000' AS DateTime2), 291281.2, 16.82, 1, CAST(N'2023-01-23T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2699, 33, N'1027', 113, CAST(N'2023-06-20T00:00:00.0000000' AS DateTime2), 99250.83, 5.16, 1, CAST(N'2023-01-30T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2700, 33, N'1028', 113, CAST(N'2023-07-20T00:00:00.0000000' AS DateTime2), 241978.82, 12.16, 1, CAST(N'2023-01-24T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2701, 33, N'1029', 113, CAST(N'2023-08-20T00:00:00.0000000' AS DateTime2), 658770.35, 32.69, 2, CAST(N'2023-01-24T00:00:00.0000000' AS DateTime2), CAST(N'2023-01-25T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2702, 34, N'1030', 113, CAST(N'2023-09-20T00:00:00.0000000' AS DateTime2), 500000, 31.25, 2, CAST(N'2023-01-21T00:00:00.0000000' AS DateTime2), CAST(N'2023-01-23T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2703, 37, N'1031', 116, CAST(N'2023-04-01T00:00:00.0000000' AS DateTime2), 266099.55, 35, 3, CAST(N'2023-05-02T00:00:00.0000000' AS DateTime2), CAST(N'2023-05-04T00:00:00.0000000' AS DateTime2), CAST(N'2023-05-30T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2704, 37, N'1032', 116, CAST(N'2023-05-01T00:00:00.0000000' AS DateTime2), 581389.09, 40, 3, CAST(N'2022-06-08T00:00:00.0000000' AS DateTime2), CAST(N'2023-06-12T00:00:00.0000000' AS DateTime2), CAST(N'2023-06-15T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2706, 37, N'1034', 116, CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), 447631.66, 24.83, 3, CAST(N'2023-07-06T00:00:00.0000000' AS DateTime2), CAST(N'2023-07-07T00:00:00.0000000' AS DateTime2), CAST(N'2023-08-01T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2707, 39, N'1037', 118, CAST(N'2023-02-25T00:00:00.0000000' AS DateTime2), 25461.75, 0.73, 2, CAST(N'2023-02-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-02T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2708, 39, N'1038', 118, CAST(N'2023-03-25T00:00:00.0000000' AS DateTime2), 1474538.25, 48.9, 2, CAST(N'2023-01-31T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-02T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2709, 40, N'1039', 118, CAST(N'2023-04-25T00:00:00.0000000' AS DateTime2), 1300000, 35.91, 1, CAST(N'2023-01-28T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2710, 43, N'1040', 121, CAST(N'2023-03-15T00:00:00.0000000' AS DateTime2), 172957.13, 18, 2, CAST(N'2023-02-21T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-23T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2711, 43, N'1041', 122, CAST(N'2023-04-15T00:00:00.0000000' AS DateTime2), 327042.87, 32.04, 1, CAST(N'2023-02-17T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2712, 44, N'1042', 122, CAST(N'2023-05-15T00:00:00.0000000' AS DateTime2), 400000, 32.88, 3, CAST(N'2023-02-24T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-26T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-27T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2714, 45, N'1044', 123, CAST(N'2023-05-01T00:00:00.0000000' AS DateTime2), 840821.09, 44, 3, CAST(N'2023-03-13T00:00:00.0000000' AS DateTime2), CAST(N'2023-04-03T00:00:00.0000000' AS DateTime2), CAST(N'2023-04-28T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2715, 46, N'1045', 123, CAST(N'2023-06-01T00:00:00.0000000' AS DateTime2), 50136.19, 2.71, 3, CAST(N'2023-03-19T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-21T00:00:00.0000000' AS DateTime2), CAST(N'2023-04-03T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2716, 46, N'1046', 123, CAST(N'2023-07-01T00:00:00.0000000' AS DateTime2), 92870.67, 5, 3, CAST(N'2023-08-07T00:00:00.0000000' AS DateTime2), CAST(N'2023-08-10T00:00:00.0000000' AS DateTime2), CAST(N'2023-08-23T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2717, 46, N'1047', 123, CAST(N'2023-08-01T00:00:00.0000000' AS DateTime2), 808092.65, 47, 3, CAST(N'2023-09-13T00:00:00.0000000' AS DateTime2), CAST(N'2023-09-15T00:00:00.0000000' AS DateTime2), CAST(N'2023-09-27T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2719, 49, N'1049', 126, CAST(N'2023-03-01T00:00:00.0000000' AS DateTime2), 1900000, 74, 3, CAST(N'2023-03-02T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-04T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-06T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2720, 51, N'1052', 128, CAST(N'2023-04-05T00:00:00.0000000' AS DateTime2), 382222.37, 19.85, 2, CAST(N'2023-03-09T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-10T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2721, 51, N'1053', 127, CAST(N'2023-05-05T00:00:00.0000000' AS DateTime2), 241006.89, 10.6, 2, CAST(N'2023-03-13T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-15T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2722, 51, N'1054', 128, CAST(N'2023-06-05T00:00:00.0000000' AS DateTime2), 84555.95, 4.82, 1, CAST(N'2023-03-11T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2723, 51, N'1055', 128, CAST(N'2023-07-05T00:00:00.0000000' AS DateTime2), 892214.79, 48.62, 2, CAST(N'2023-03-07T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-08T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2724, 53, N'1059', 129, CAST(N'2023-02-15T00:00:00.0000000' AS DateTime2), 65891.46, 1.98, 1, CAST(N'2023-01-17T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2725, 53, N'1060', 129, CAST(N'2023-03-15T00:00:00.0000000' AS DateTime2), 249919.07, 7.01, 1, CAST(N'2023-01-24T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2726, 53, N'1061', 129, CAST(N'2023-04-15T00:00:00.0000000' AS DateTime2), 2184189.47, 69.85, 1, CAST(N'2023-01-22T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2727, 55, N'1066', 130, CAST(N'2023-03-25T00:00:00.0000000' AS DateTime2), 34842.6, 1.56, 2, CAST(N'2023-03-03T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-05T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2728, 55, N'1067', 131, CAST(N'2023-04-25T00:00:00.0000000' AS DateTime2), 865157.4, 44.93, 1, CAST(N'2023-02-28T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2729, 56, N'1068', 130, CAST(N'2023-05-25T00:00:00.0000000' AS DateTime2), 277794.73, 14.97, 1, CAST(N'2023-03-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2730, 56, N'1069', 130, CAST(N'2023-06-25T00:00:00.0000000' AS DateTime2), 67516.74, 2.81, 1, CAST(N'2023-03-05T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2731, 56, N'1070', 130, CAST(N'2023-07-25T00:00:00.0000000' AS DateTime2), 554688.53, 26.27, 3, CAST(N'2023-03-03T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-07T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2732, 57, N'1071', 130, CAST(N'2023-04-10T00:00:00.0000000' AS DateTime2), 3500000, 98.84, 2, CAST(N'2023-03-19T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-20T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2733, 59, N'1075', 133, CAST(N'2023-02-28T00:00:00.0000000' AS DateTime2), 227760.77, 8.31, 2, CAST(N'2023-01-31T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-02T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2734, 59, N'1076', 133, CAST(N'2023-03-28T00:00:00.0000000' AS DateTime2), 299023.35, 11.5, 2, CAST(N'2023-01-31T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-02T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2735, 59, N'1077', 134, CAST(N'2023-04-28T00:00:00.0000000' AS DateTime2), 873215.88, 31.22, 1, CAST(N'2023-02-02T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2736, 60, N'1078', 134, CAST(N'2023-05-28T00:00:00.0000000' AS DateTime2), 700000, 26.92, 3, CAST(N'2023-02-06T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-07T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-09T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2737, 61, N'1079', 135, CAST(N'2023-03-10T00:00:00.0000000' AS DateTime2), 281870.82, 29.77, 1, CAST(N'2023-02-12T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2738, 61, N'1080', 135, CAST(N'2023-04-10T00:00:00.0000000' AS DateTime2), 518129.18, 49.78, 2, CAST(N'2023-02-19T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-21T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2739, 63, N'1082', 137, CAST(N'2023-03-15T00:00:00.0000000' AS DateTime2), 380368.66, 28.28, 1, CAST(N'2023-02-24T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2740, 63, N'1083', 137, CAST(N'2023-04-15T00:00:00.0000000' AS DateTime2), 8398.58, 0.57, 1, CAST(N'2023-02-24T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2741, 63, N'1084', 137, CAST(N'2023-05-15T00:00:00.0000000' AS DateTime2), 811232.76, 51.24, 1, CAST(N'2023-02-23T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2742, 65, N'1089', 139, CAST(N'2023-03-20T00:00:00.0000000' AS DateTime2), 236537.81, 6.51, 1, CAST(N'2023-03-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2743, 65, N'1090', 138, CAST(N'2023-04-20T00:00:00.0000000' AS DateTime2), 635937.37, 17.5, 1, CAST(N'2023-02-27T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2744, 65, N'1091', 139, CAST(N'2023-05-20T00:00:00.0000000' AS DateTime2), 250431.97, 8.23, 1, CAST(N'2023-02-25T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2745, 65, N'1092', 139, CAST(N'2023-06-20T00:00:00.0000000' AS DateTime2), 1677092.85, 57.18, 1, CAST(N'2023-02-26T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2746, 67, N'1096', 141, CAST(N'2023-04-05T00:00:00.0000000' AS DateTime2), 9890.9, 0.46, 2, CAST(N'2023-03-12T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-13T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2747, 67, N'1097', 140, CAST(N'2023-05-05T00:00:00.0000000' AS DateTime2), 345566.56, 16.87, 1, CAST(N'2023-03-15T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2748, 67, N'1098', 140, CAST(N'2023-06-05T00:00:00.0000000' AS DateTime2), 1544542.54, 61.62, 3, CAST(N'2023-03-15T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-16T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-17T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2749, 69, N'1100', 142, CAST(N'2023-02-25T00:00:00.0000000' AS DateTime2), 1500000, 86.67, 2, CAST(N'2023-01-27T00:00:00.0000000' AS DateTime2), CAST(N'2023-01-28T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2750, 71, N'1104', 143, CAST(N'2023-02-20T00:00:00.0000000' AS DateTime2), 129347.23, 7.22, 1, CAST(N'2023-01-23T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2751, 71, N'1105', 143, CAST(N'2023-03-20T00:00:00.0000000' AS DateTime2), 390736.65, 18.73, 2, CAST(N'2023-01-22T00:00:00.0000000' AS DateTime2), CAST(N'2023-01-24T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2752, 71, N'1106', 144, CAST(N'2023-04-20T00:00:00.0000000' AS DateTime2), 341405.8, 15.76, 2, CAST(N'2023-01-30T00:00:00.0000000' AS DateTime2), CAST(N'2023-01-31T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2753, 71, N'1107', 144, CAST(N'2023-05-20T00:00:00.0000000' AS DateTime2), 838510.32, 39.31, 1, CAST(N'2023-01-24T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2754, 73, N'1109', 146, CAST(N'2023-04-10T00:00:00.0000000' AS DateTime2), 299231.56, 14.42, 3, CAST(N'2023-03-19T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-21T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-23T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2755, 73, N'1110', 145, CAST(N'2023-05-10T00:00:00.0000000' AS DateTime2), 331895.35, 16.02, 3, CAST(N'2023-03-15T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-16T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-18T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2756, 73, N'1111', 145, CAST(N'2023-06-10T00:00:00.0000000' AS DateTime2), 38894.8, 1.57, 1, CAST(N'2023-03-13T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2757, 73, N'1112', 145, CAST(N'2023-07-10T00:00:00.0000000' AS DateTime2), 1329978.29, 58.14, 1, CAST(N'2023-03-15T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2758, 75, N'1115', 146, CAST(N'2023-02-28T00:00:00.0000000' AS DateTime2), 2500000, 74.72, 3, CAST(N'2023-01-31T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-02T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-03T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2759, 76, N'1116', 148, CAST(N'2023-03-28T00:00:00.0000000' AS DateTime2), 84552.85, 2.59, 1, CAST(N'2023-02-07T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2760, 76, N'1117', 147, CAST(N'2023-04-28T00:00:00.0000000' AS DateTime2), 415447.15, 13.72, 1, CAST(N'2023-02-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2761, 77, N'1118', 149, CAST(N'2023-03-25T00:00:00.0000000' AS DateTime2), 403231.95, 17.88, 1, CAST(N'2023-03-04T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2762, 77, N'1119', 149, CAST(N'2023-04-25T00:00:00.0000000' AS DateTime2), 445916.29, 16.32, 1, CAST(N'2023-03-04T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2763, 77, N'1120', 149, CAST(N'2023-05-25T00:00:00.0000000' AS DateTime2), 750851.76, 30.59, 2, CAST(N'2023-02-27T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2764, 78, N'1121', 149, CAST(N'2023-06-25T00:00:00.0000000' AS DateTime2), 105574.8, 4.52, 1, CAST(N'2023-03-05T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2765, 78, N'1122', 149, CAST(N'2023-07-25T00:00:00.0000000' AS DateTime2), 21459.59, 0.91, 3, CAST(N'2023-03-07T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-09T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-11T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2766, 78, N'1123', 149, CAST(N'2023-08-25T00:00:00.0000000' AS DateTime2), 115845.37, 4.29, 3, CAST(N'2023-03-05T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-07T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-09T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2767, 78, N'1124', 149, CAST(N'2023-09-25T00:00:00.0000000' AS DateTime2), 357120.24, 11.55, 1, CAST(N'2023-03-07T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2768, 79, N'1125', 150, CAST(N'2023-04-05T00:00:00.0000000' AS DateTime2), 900000, 47.3, 1, CAST(N'2023-03-12T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2769, 80, N'1126', 151, CAST(N'2023-05-05T00:00:00.0000000' AS DateTime2), 134513.55, 5.44, 1, CAST(N'2023-03-12T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2770, 80, N'1127', 151, CAST(N'2023-06-05T00:00:00.0000000' AS DateTime2), 289310.62, 11.9, 1, CAST(N'2023-03-11T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2771, 80, N'1128', 151, CAST(N'2023-07-05T00:00:00.0000000' AS DateTime2), 576175.83, 23, 2, CAST(N'2023-03-14T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-16T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2772, 81, N'1129', 151, CAST(N'2023-02-25T00:00:00.0000000' AS DateTime2), 1400000, 92.98, 1, CAST(N'2023-02-01T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2773, 83, N'1133', 153, CAST(N'2023-03-20T00:00:00.0000000' AS DateTime2), 2500000, 94.14, 1, CAST(N'2023-02-25T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2774, 85, N'1136', 156, CAST(N'2023-03-10T00:00:00.0000000' AS DateTime2), 900000, 90.95, 2, CAST(N'2023-02-12T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-14T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2775, 87, N'1141', 156, CAST(N'2023-03-15T00:00:00.0000000' AS DateTime2), 1700000, 80.9, 1, CAST(N'2023-02-23T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2776, 89, N'1146', 159, CAST(N'2023-02-28T00:00:00.0000000' AS DateTime2), 251234.67, 11.16, 2, CAST(N'2023-02-07T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-09T00:00:00.0000000' AS DateTime2), NULL, NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2777, 89, N'1147', 159, CAST(N'2023-03-28T00:00:00.0000000' AS DateTime2), 1848765.33, 81.91, 3, CAST(N'2023-01-31T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-01T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-02T00:00:00.0000000' AS DateTime2), NULL, NULL)
INSERT [dbo].[Payment] ([Id], [Product], [Code], [FundingSource], [ReportedMonth], [PaymentAmount], [PhysicalAdvance], [Stage], [DateDelivery], [DateApproved], [DatePayed], [AttachmentAdvance], [AttachmentPayment]) VALUES (2779, 49, N'03779', 126, CAST(N'2023-09-01T00:00:00.0000000' AS DateTime2), 1000000, 26, 3, CAST(N'2023-10-02T00:00:00.0000000' AS DateTime2), CAST(N'2023-10-16T00:00:00.0000000' AS DateTime2), CAST(N'2023-10-30T00:00:00.0000000' AS DateTime2), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Payment] OFF
GO
SET IDENTITY_INSERT [dbo].[PaymentStage] ON 

INSERT [dbo].[PaymentStage] ([Id], [Title], [SortOrder]) VALUES (1, N'Presentado', 1)
INSERT [dbo].[PaymentStage] ([Id], [Title], [SortOrder]) VALUES (2, N'Aprobado', 2)
INSERT [dbo].[PaymentStage] ([Id], [Title], [SortOrder]) VALUES (3, N'Pagado', 3)
SET IDENTITY_INSERT [dbo].[PaymentStage] OFF
GO
SET IDENTITY_INSERT [dbo].[Product] ON 

INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (13, 1, N'Área Recreativa Mejorada', 400000, N'Renovación y modernización de las áreas recreativas.', N'Mejorar la calidad de vida de los ciudadanos.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (14, 1, N'Nuevas Zonas Verdes', 100000, N'Creación de nuevas zonas verdes y espacios naturales.', N'Fomentar la sostenibilidad ambiental.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (15, 2, N'Construcción de Edificio Escolar', 200000, N'Construcción de un nuevo edificio escolar para primaria.', N'Proporcionar instalaciones educativas adecuadas.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (16, 2, N'Equipamiento Escolar', 1000000, N'Compra de equipamiento educativo para la escuela.', N'Garantizar recursos para una educación de calidad.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (17, 3, N'Ampliación de Salas', 1500000, N'Ampliación y remodelación de salas hospitalarias existentes.', N'Mejorar la capacidad y el confort del hospital.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (18, 3, N'Equipamiento Médico', 500000, N'Adquisición de equipos médicos de última generación.', N'Optimizar la atención médica.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (19, 4, N'Construcción de Instalaciones Deportivas', 500000, N'Construcción de instalaciones deportivas múltiples.', N'Promover el deporte y la actividad física.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (20, 4, N'Área de Recreación y Juegos', 300000, N'Creación de áreas de recreación y juegos infantiles.', N'Fomentar el desarrollo físico y social de los niños.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (21, 5, N'Ampliación del paseo peatonal del malecón turístico', 100000, N'Ampliación del paseo peatonal.', N'Promover el deporte en los espacios públicos.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (22, 5, N'Ciclovía del Malecón Turístico', 200000, N'Creación de una ciclovía en el malecón turístico.', N'Generación de espacios para la recreación y el deporte.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (23, 6, N'Reparación de Calles Principales', 500000, N'Reparación y asfaltado de calles principales.', N'Mejorar la infraestructura vial para facilitar el tráfico.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (24, 6, N'Ampliación de Vías Peatonales', 500000, N'Creación de nuevas vías peatonales y ciclovías.', N'Fomentar el transporte sostenible.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (25, 7, N'Construcción de Viviendas', 1000000, N'Construcción de viviendas sociales para familias necesitadas.', N'Proporcionar vivienda digna y accesible.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (26, 7, N'Áreas Comunales', 500000, N'Creación de áreas comunes y espacios públicos.', N'Promover la convivencia y el desarrollo comunitario.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (27, 8, N'Infraestructura de Agua Potable', 300000, N'Instalación de sistemas de agua potable en comunidades rurales.', N'Mejorar el acceso al agua potable.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (28, 8, N'Saneamiento Básico', 400000, N'Mejora del sistema de saneamiento básico en áreas rurales.', N'Promover la salud pública y la higiene.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (29, 9, N'Apoyo Financiero a Pequeños Comercios', 300000, N'Apoyo financiero directo a pequeños comerciantes.', N'Impulsar la economía local y el empleo.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (30, 9, N'Capacitación Empresarial', 200000, N'Programas de capacitación y formación empresarial.', N'Fortalecer las capacidades empresariales de los comerciantes.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (31, 10, N'Modernización del Alumbrado Público', 200000, N'Modernización y eficiencia energética del alumbrado público.', N'Aumentar la seguridad y mejorar la iluminación urbana.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (32, 100, N'Adquisición de libros y material didáctico', 500000, N'Compra de libros y material didáctico para la biblioteca.', N'Mejorar el acceso a la educación y cultura en la comunidad.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (33, 100, N'Equipamiento tecnológico para sala multimedia', 1000000, N'Instalación de equipos multimedia para la sala de lectura.', N'Facilitar el acceso a recursos digitales y multimedia.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (34, 100, N'Mobiliario para salas de lectura y estudio', 500000, N'Compra de mesas, sillas y estanterías para las áreas de lectura.', N'Crear espacios cómodos y funcionales para usuarios.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (35, 101, N'Renovación de tuberías principales', 700000, N'Reemplazo de tuberías obsoletas en áreas críticas del sistema.', N'Mejorar la eficiencia y capacidad del sistema de alcantarillado.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (36, 101, N'Construcción de nuevas estaciones de bombeo', 1400000, N'Instalación de estaciones de bombeo modernas y eficientes.', N'Optimizar el flujo y tratamiento de aguas residuales.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (37, 102, N'Talleres prácticos de desarrollo de software', 1300000, N'Organización de talleres intensivos para aprender a desarrollar software.', N'Capacitar a los participantes en habilidades de desarrollo de software.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (39, 103, N'Construcción de aulas y áreas administrativas', 1500000, N'Construcción de aulas equipadas y oficinas administrativas.', N'Proporcionar espacios adecuados para la enseñanza y administración escolar.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (40, 103, N'Instalación de laboratorios de ciencias', 1300000, N'Equipamiento de laboratorios para enseñanza de ciencias.', N'Facilitar la educación científica y experimental para los estudiantes.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (41, 104, N'Construcción de edificios de oficinas', 1750000, N'Edificación de espacios para empresas tecnológicas.', N'Crear un hub tecnológico para promover la innovación y el emprendimiento.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (42, 104, N'Desarrollo de infraestructura de telecomunicaciones', 1750000, N'Instalación de redes de comunicaciones de alta velocidad.', N'Facilitar la conectividad y acceso a tecnologías avanzadas.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (43, 105, N'Campañas educativas sobre reciclaje', 500000, N'Organización de talleres y charlas sobre la importancia del reciclaje.', N'Crear conciencia sobre la importancia del reciclaje en la comunidad escolar.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (44, 105, N'Instalación de contenedores de reciclaje', 400000, N'Compra e instalación de contenedores adecuados para reciclaje.', N'Facilitar la separación y recolección de materiales reciclables.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (45, 106, N'Remodelación de áreas verdes y paisajismo', 1000000, N'Diseño y plantación de áreas verdes y paisajismo.', N'Embellecer y mejorar el entorno urbano de la plaza principal.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (46, 106, N'Rehabilitación de la fuente central', 800000, N'Restauración y modernización de la fuente central.', N'Recuperar y preservar un elemento histórico y cultural de la plaza.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (47, 107, N'Construcción de canchas deportivas', 1100000, N'Creación de espacios para la práctica de deportes.', N'Promover la actividad física y el deporte entre los habitantes.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (48, 107, N'Instalación de equipamiento deportivo', 1100000, N'Compra e instalación de equipos deportivos modernos.', N'Dotar al centro con equipamiento adecuado para diversas disciplinas.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (49, 108, N'Renovación de flota de autobuses', 2900000, N'Adquisición de nuevos autobuses para mejorar el servicio.', N'Mejorar la calidad y eficiencia del transporte público urbano.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (51, 109, N'Capacitación en desarrollo de negocios', 1600000, N'Organización de talleres y cursos para mujeres emprendedoras.', N'Fortalecer habilidades empresariales y de gestión entre las mujeres.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (52, 109, N'Microcréditos para emprendimientos', 0, N'Otorgamiento de financiamiento a mujeres para iniciar o expandir negocios.', N'Apoyar financieramente proyectos emprendedores liderados por mujeres.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (53, 110, N'Construcción de aulas y áreas administrativas', 2500000, N'Edificación de espacios para enseñanza y administración escolar.', N'Proporcionar instalaciones educativas adecuadas para estudiantes y personal docente.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (54, 110, N'Equipamiento educativo y mobiliario escolar', 0, N'Compra e instalación de material didáctico y mobiliario para aulas.', N'Crear un entorno educativo cómodo y funcional.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (55, 111, N'Ampliación y remodelación de áreas de atención', 900000, N'Mejora de áreas de emergencia y hospitalización.', N'Incrementar la capacidad de atención y confort para pacientes y personal médico.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (56, 111, N'Adquisición de equipamiento médico especializado', 900000, N'Compra de equipos avanzados para diagnóstico y tratamiento.', N'Dotar al hospital con tecnología médica de última generación.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (57, 112, N'Construcción de tramos principales y secundarios', 3500000, N'Edificación de tramos viales fundamentales para la conectividad.', N'Mejorar la accesibilidad y comunicación entre comunidades rurales y urbanas.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (58, 112, N'Construcción de puentes y estructuras complementarias', 0, N'Edificación de infraestructura vial complementaria para mejorar la seguridad vial.', N'Facilitar el tránsito seguro de vehículos y peatones.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (59, 113, N'Entrega de paquetes alimentarios mensuales', 1400000, N'Distribución de alimentos básicos para familias vulnerables.', N'Contribuir a la seguridad alimentaria de hogares en situación de vulnerabilidad.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (60, 113, N'Programa de asistencia social y psicológica', 700000, N'Implementación de apoyo emocional y social para familias.', N'Brindar acompañamiento y orientación a familias en situaciones difíciles.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (61, 114, N'Organización de eventos culturales y artísticos', 800000, N'Planificación y ejecución de actividades culturales.', N'Promover la cultura y el arte entre los residentes y visitantes de la comunidad.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (62, 114, N'Instalación de infraestructura temporal para eventos', 0, N'Montaje de estructuras para espectáculos y exposiciones.', N'Crear espacios adecuados para la celebración de eventos culturales.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (63, 115, N'Plantación de árboles nativos y mantenimiento', 1200000, N'Siembra de especies vegetales autóctonas y cuidado del entorno.', N'Fomentar la conservación y restauración del ecosistema local.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (64, 115, N'Educación ambiental y talleres de sensibilización', 0, N'Realización de actividades educativas sobre la importancia de la reforestación.', N'Concienciar a la comunidad sobre la protección del medio ambiente.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (65, 116, N'Programa de formación y capacitación laboral', 2800000, N'Implementación de cursos y talleres para desarrollo profesional.', N'Preparar a jóvenes para el mercado laboral y mejorar sus oportunidades de empleo.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (66, 116, N'Creación de pasantías y oportunidades laborales', 0, N'Generación de oportunidades de empleo y prácticas laborales para jóvenes.', N'Facilitar la inserción laboral de jóvenes en diversos sectores productivos.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (67, 117, N'Equipamiento de seguridad y tecnología policial', 1900000, N'Adquisición de equipos modernos para la policía local.', N'Mejorar la capacidad de respuesta y operatividad de las fuerzas de seguridad.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (68, 117, N'Capacitación en técnicas de seguridad y prevención del delito', 0, N'Organización de cursos y talleres para mejorar habilidades de seguridad.', N'Fortalecer las capacidades de la policía en la prevención y control del delito.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (69, 118, N'Actualización de software y plataformas contables', 1500000, N'Adquisición e implementación de sistemas de gestión contable.', N'Optimizar la administración financiera y contable del gobierno municipal.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (70, 118, N'Capacitación en técnicas y normativas contables', 0, N'Organización de cursos para mejorar las habilidades contables del personal.', N'Fortalecer la capacidad del personal en la gestión financiera y contable.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (71, 119, N'Talleres prácticos sobre elaboración de presupuestos', 1700000, N'Organización de talleres para aprender a elaborar presupuestos.', N'Capacitar a funcionarios en la planificación y gestión financiera pública.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (72, 119, N'Seminarios sobre administración financiera gubernamental', 0, N'Realización de seminarios para profundizar en temas de gestión financiera.', N'Mejorar la eficiencia y transparencia en la administración de recursos públicos.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (73, 120, N'Adquisición de equipos de protección personal', 2000000, N'Compra de equipos para garantizar la seguridad de los bomberos.', N'Mejorar la capacidad de respuesta ante emergencias y desastres.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (74, 120, N'Actualización de vehículos y maquinaria de rescate', 0, N'Renovación de vehículos y maquinaria especializada para operaciones de rescate.', N'Optimizar las operaciones de rescate y salvamento.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (75, 121, N'Construcción de viviendas con materiales ecológicos', 2500000, N'Edificación de casas utilizando materiales sostenibles y eficientes.', N'Promover la construcción responsable y respetuosa con el medio ambiente.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (76, 121, N'Instalación de sistemas de energía renovable', 500000, N'Implementación de tecnologías para el aprovechamiento de energías limpias.', N'Reducir el impacto ambiental y los costos energéticos de las viviendas.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (77, 122, N'Instalación de paneles solares en edificios municipales', 1600000, N'Adaptación de edificios para generar energía solar.', N'Reducir la huella de carbono y los costos energéticos del gobierno municipal.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (78, 122, N'Modernización de sistemas de iluminación eficiente', 600000, N'Cambio de sistemas de iluminación por tecnología LED.', N'Promover el uso responsable de la energía en edificaciones públicas.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (79, 123, N'Promoción y marketing digital para comercios', 900000, N'Difusión y promoción de negocios locales a través de plataformas digitales.', N'Impulsar la visibilidad y ventas de los comercios locales en el mercado digital.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (80, 123, N'Capacitación en estrategias de ventas y gestión empresarial', 1000000, N'Organización de cursos para mejorar habilidades de gestión empresarial.', N'Fortalecer la competitividad y sostenibilidad de los negocios locales.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (81, 124, N'Restauración de salas de exposiciones y auditorio', 1400000, N'Rehabilitación de espacios para actividades culturales.', N'Preservar y promover la cultura local a través de eventos y exposiciones.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (82, 124, N'Adquisición de equipamiento audiovisual', 0, N'Compra de equipos para mejorar las capacidades audiovisuales del centro cultural.', N'Mejorar la calidad de las presentaciones y eventos culturales.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (83, 125, N'Restauración de hábitats acuáticos y áreas de anidación', 2500000, N'Rehabilitación de áreas naturales para proteger especies vulnerables.', N'Conservar la biodiversidad y los ecosistemas acuáticos de la región.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (84, 125, N'Educación ambiental y sensibilización comunitaria', 0, N'Organización de actividades educativas sobre la importancia de los humedales.', N'Fomentar el conocimiento y respeto por los humedales entre la comunidad.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (85, 126, N'Organización de festivales y conciertos', 900000, N'Planificación y ejecución de eventos culturales y musicales.', N'Fomentar la cultura y el arte a través de eventos públicos.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (86, 126, N'Exposiciones de arte y muestras culturales', 0, N'Montaje de exposiciones y muestras de arte en espacios públicos.', N'Promover la apreciación y difusión de las expresiones culturales.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (87, 127, N'Restauración de edificaciones históricas', 1000000, N'Rehabilitación de estructuras arquitectónicas antiguas.', N'Preservar y proteger el patrimonio histórico y cultural del municipio.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (88, 127, N'Mantenimiento de áreas públicas históricas', 700000, N'Conservación y limpieza de áreas y monumentos históricos.', N'Mantener accesibles y en buenas condiciones los sitios de interés histórico.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (89, 128, N'Rehabilitación de áreas de atención médica', 2100000, N'Mejora de salas de espera y consultorios médicos.', N'Ofrecer instalaciones cómodas y eficientes para la atención de salud comunitaria.')
INSERT [dbo].[Product] ([Id], [Project], [Name], [Cost], [Description], [Objective]) VALUES (90, 128, N'Adquisición de equipamiento médico especializado', 0, N'Compra de equipos avanzados para diagnóstico y tratamiento.', N'Dotar al centro con tecnología médica de última generación.')
SET IDENTITY_INSERT [dbo].[Product] OFF
GO
SET IDENTITY_INSERT [dbo].[Project] ON 

INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (1, N'Renovación del Parque Central', N'p0001', 5, 10, 1, 1, N'Renovación y mejoramiento del Parque Central para fomentar actividades recreativas y culturales.', N'Mejorar la calidad de vida de los ciudadanos mediante la renovación de espacios públicos.', 500000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.446909,11.777676]}}]}', NULL, NULL, 360, CAST(N'2023-01-01T00:00:00.0000000' AS DateTime2), 11, 1)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (2, N'Construcción de Escuela Primaria Las Nubes', N'p0002', 1, 1, 2, 2, N'Construcción de una nueva escuela primaria en el barrio San Juan.', N'Proveer educación de calidad y accesible para todos los niños del barrio.', 1200000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.448051,11.766343]}}]}', NULL, CAST(N'2023-06-01T00:00:00.0000000' AS DateTime2), 540, CAST(N'2023-03-01T00:00:00.0000000' AS DateTime2), 4, 1)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (3, N'Mejoramiento de la Infraestructura Hospitalaria', N'p0003', 2, 4, 1, 3, N'Modernización y ampliación del hospital general para mejorar los servicios de salud.', N'Garantizar atención médica oportuna y de calidad.', 2000000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.4450,11.7765]}}]}', NULL, NULL, 540, CAST(N'2023-01-01T00:00:00.0000000' AS DateTime2), 3, 1)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (4, N'Construcción de Centro Deportivo', N'p0004', 3, 7, 2, 4, N'Construcción de un centro deportivo multifuncional para fomentar el deporte.', N'Promover la actividad física y el deporte en la comunidad.', 800000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.442794,11.768851]}}]}', NULL, CAST(N'2023-09-01T00:00:00.0000000' AS DateTime2), 450, CAST(N'2023-05-01T00:00:00.0000000' AS DateTime2), 3, 1)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (5, N'Ampliación del Malecón Turístico', N'p0005', 7, 15, 1, 8, N'Ampliación de las zonas peatonales y deportivas del malecón.', N'Ofrecer espacios de esparcimiento para turistas y residentes.', 300000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.449487,11.777507]}},{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.447154,11.778003]}},{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.447309,11.774572]}}]}', NULL, NULL, 360, CAST(N'2024-11-01T00:00:00.0000000' AS DateTime2), 8, 6)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (6, N'Mejoramiento de Vías Urbanas', N'p0006', 3, 6, 2, 6, N'Repavimentación y mantenimiento de calles en el centro urbano.', N'Mejorar la infraestructura vial para facilitar la movilidad y reducir los accidentes.', 1000000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"LineString","coordinates":[[-72.435293,11.779765],[-72.435007,11.77351]]}}]}', NULL, CAST(N'2023-11-01T00:00:00.0000000' AS DateTime2), 390, CAST(N'2023-02-01T00:00:00.0000000' AS DateTime2), 9, 1)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (7, N'Construcción de Viviendas Sociales', N'p0007', 4, 8, 2, 7, N'Construcción de viviendas para familias de bajos ingresos.', N'Reducir el déficit habitacional y mejorar la calidad de vida.', 1500000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Polygon","coordinates":[[[-72.435122,11.77611],[-72.434907,11.772263],[-72.431946,11.772347],[-72.432075,11.77632],[-72.435122,11.77611]]]}}]}', NULL, CAST(N'2023-10-01T00:00:00.0000000' AS DateTime2), 420, CAST(N'2023-04-01T00:00:00.0000000' AS DateTime2), 1, 1)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (8, N'Instalación de Sistema de Agua Potable en Zonas Rurales', N'p0008', 6, NULL, 1, 8, N'Implementación de un sistema de agua potable en comunidades rurales.', N'Garantizar el acceso a agua potable y mejorar la salud pública.', 700000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"LineString","coordinates":[[-72.42671,11.7592],[-72.425938,11.762732],[-72.424135,11.763825],[-72.426023,11.767945],[-72.425852,11.770804],[-72.419329,11.772654],[-72.419586,11.776185]]}}]}', NULL, NULL, 360, CAST(N'2023-01-01T00:00:00.0000000' AS DateTime2), 6, 1)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (9, N'Programa de Apoyo a Pequeños Comerciantes', N'p0009', 7, NULL, 2, 9, N'Capacitación y financiamiento para pequeños comerciantes locales.', N'Fortalecer la economía local y crear empleo.', 500000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.442177,11.773793]}}]}', NULL, CAST(N'2023-08-01T00:00:00.0000000' AS DateTime2), 480, CAST(N'2023-03-01T00:00:00.0000000' AS DateTime2), 8, 1)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (10, N'Mejoramiento del Alumbrado Público', N'p0010', 3, 6, 2, 10, N'Instalación de nuevas luminarias LED en las principales avenidas.', N'Reducir el consumo energético y mejorar la seguridad.', 400000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"LineString","coordinates":[[-72.448997,11.777315],[-72.448483,11.775569],[-72.446315,11.776021],[-72.446562,11.778386],[-72.441788,11.778742]]}}]}', NULL, CAST(N'2023-09-01T00:00:00.0000000' AS DateTime2), 450, CAST(N'2023-01-01T00:00:00.0000000' AS DateTime2), 7, 1)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (100, N'Construcción de la Biblioteca Pública El Bosque', N'P0100', 1, 1, 2, 2, N'Construcción de una nueva biblioteca pública en el barrio El Bosque', N'Fomentar la lectura y el acceso a la información', 1500000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.438724,11.775656]}}]}', NULL, CAST(N'2023-01-20T00:00:00.0000000' AS DateTime2), 690, CAST(N'2023-01-05T00:00:00.0000000' AS DateTime2), 4, 2)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (101, N'Mejora del Sistema de Alcantarillado en Los Pinos', N'P0101', 6, 12, 1, 7, N'Mejora y expansión del sistema de alcantarillado en el barrio Los Pinos', N'Garantizar un sistema de alcantarillado eficiente y sostenible', 2100000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"LineString","coordinates":[[-72.433442,11.770516],[-72.433864,11.76897],[-72.433342,11.767612],[-72.432568,11.767648]]}}]}', NULL, CAST(N'2023-02-10T00:00:00.0000000' AS DateTime2), 180, CAST(N'2023-01-25T00:00:00.0000000' AS DateTime2), 6, 7)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (102, N'Programa de Capacitación en Tecnologías de Información', N'P0102', 7, 15, 3, 8, N'Capacitación en tecnologías de información para jóvenes y adultos', N'Incrementar la empleabilidad y el desarrollo tecnológico', 1300000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.439892,11.775214]}}]}', CAST(N'2023-11-30T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-01T00:00:00.0000000' AS DateTime2), 220, CAST(N'2023-02-15T00:00:00.0000000' AS DateTime2), 8, 8)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (103, N'Construcción de la Escuela Secundaria Los Laureles', N'P0103', 1, 1, 2, 2, N'Construcción de una nueva escuela secundaria en Los Laureles', N'Mejorar el acceso a la educación secundaria de calidad', 2800000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Polygon","coordinates":[[[-72.432931,11.769376],[-72.432915,11.768913],[-72.432299,11.768912],[-72.432315,11.769387],[-72.432931,11.769376]]]}}]}', NULL, CAST(N'2023-01-25T00:00:00.0000000' AS DateTime2), 690, CAST(N'2023-01-10T00:00:00.0000000' AS DateTime2), 4, 2)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (104, N'Creación del Parque Tecnológico Innovación', N'P0104', 7, 14, 1, 8, N'Creación de un parque tecnológico para fomentar la innovación y el emprendimiento', N'Promover el desarrollo tecnológico y la competitividad', 3500000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.431715,11.772814]}}]}', NULL, CAST(N'2023-02-05T00:00:00.0000000' AS DateTime2), 260, CAST(N'2023-01-20T00:00:00.0000000' AS DateTime2), 9, 8)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (105, N'Implementación de Programas de Reciclaje en Escuelas', N'P0105', 6, 12, 2, 7, N'Implementación de programas de reciclaje en escuelas primarias y secundarias', N'Fomentar la conciencia ambiental y el reciclaje entre los estudiantes', 900000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.429711,11.773961]}}]}', NULL, CAST(N'2023-02-15T00:00:00.0000000' AS DateTime2), 660, CAST(N'2023-01-30T00:00:00.0000000' AS DateTime2), 12, 7)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (106, N'Renovación de la Plaza Principal de Villa del Sol', N'P0106', 3, 7, 3, 4, N'Renovación y modernización de la plaza principal en Villa del Sol', N'Crear un espacio de esparcimiento y convivencia comunitaria', 1800000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.447898,11.776094]}}]}', CAST(N'2023-10-31T00:00:00.0000000' AS DateTime2), CAST(N'2023-03-10T00:00:00.0000000' AS DateTime2), 230, CAST(N'2023-02-25T00:00:00.0000000' AS DateTime2), 11, 4)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (107, N'Construcción del Centro Deportivo La Esperanza', N'P0107', 5, 10, 1, 6, N'Construcción de un nuevo centro deportivo en La Esperanza', N'Fomentar la actividad física y el deporte en la comunidad', 2200000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.444704,11.774466]}}]}', NULL, CAST(N'2023-01-30T00:00:00.0000000' AS DateTime2), 250, CAST(N'2023-01-15T00:00:00.0000000' AS DateTime2), 11, 6)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (108, N'Mejora del Sistema de Transporte Público en San Rafael', N'P0108', 8, 16, 3, 9, N'Mejora y modernización del sistema de transporte público en San Rafael', N'Garantizar un transporte público eficiente y accesible', 2900000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"LineString","coordinates":[[-72.441802,11.766472],[-72.445235,11.7687],[-72.446351,11.778412],[-72.432017,11.780388],[-72.431974,11.769961]]}}]}', CAST(N'2023-11-30T00:00:00.0000000' AS DateTime2), CAST(N'2023-02-20T00:00:00.0000000' AS DateTime2), 240, CAST(N'2023-02-05T00:00:00.0000000' AS DateTime2), 9, 9)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (109, N'Programa de Apoyo a Mujeres Emprendedoras', N'P0109', 4, 8, 2, 5, N'Programa para apoyar a mujeres emprendedoras con capacitación y recursos', N'Impulsar el emprendimiento y la autonomía económica de las mujeres', 1600000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.445653,11.776154]}}]}', NULL, CAST(N'2023-03-05T00:00:00.0000000' AS DateTime2), 630, CAST(N'2023-02-15T00:00:00.0000000' AS DateTime2), 5, 9)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (110, N'Construcción de la Escuela Primaria San Martín', N'p0110', 1, 1, 2, 2, N'Construcción de una nueva escuela primaria en San Martín', N'Aumentar el acceso a la educación primaria de calidad', 2500000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Polygon","coordinates":[[[-72.433862,11.76988],[-72.433793,11.769235],[-72.432982,11.769337],[-72.433015,11.769949],[-72.433862,11.76988]]]}}]}', NULL, CAST(N'2023-01-15T00:00:00.0000000' AS DateTime2), 690, CAST(N'2023-01-01T00:00:00.0000000' AS DateTime2), 4, 2)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (111, N'Renovación del Hospital General de Santa María', N'p0111', 2, 4, 2, 3, N'Renovación del Hospital General de Santa María para mejorar la atención médica', N'Mejorar la infraestructura hospitalaria y servicios de salud', 1800000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.448269,11.774547]}}]}', NULL, CAST(N'2023-02-25T00:00:00.0000000' AS DateTime2), 660, CAST(N'2023-02-01T00:00:00.0000000' AS DateTime2), 3, 3)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (112, N'Construcción de la Carretera Rural El Roble', N'p0112', 3, 6, 2, 4, N'Construcción de una carretera rural en El Roble para mejorar la conectividad', N'Mejorar el acceso a comunidades rurales', 3500000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"LineString","coordinates":[[-72.374954,11.769426],[-72.373581,11.790991],[-72.371178,11.803771],[-72.347832,11.811505]]}}]}', NULL, CAST(N'2023-03-10T00:00:00.0000000' AS DateTime2), 630, CAST(N'2023-03-01T00:00:00.0000000' AS DateTime2), 8, 6)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (113, N'Apoyo a Familias Vulnerables en San Pedro', N'p0113', 4, 8, 2, 5, N'Programa para apoyar a familias vulnerables en San Pedro con viviendas sociales', N'Reducir la pobreza y mejorar las condiciones de vida', 2100000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.44942,11.778341]}}]}', NULL, CAST(N'2023-01-30T00:00:00.0000000' AS DateTime2), 690, CAST(N'2023-01-15T00:00:00.0000000' AS DateTime2), 1, 8)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (114, N'Festival de Artes y Cultura en Plaza Central', N'p0114', 5, 10, 2, 6, N'Organización de un festival anual de artes y cultura en la Plaza Central', N'Fomentar la participación cultural y artística', 800000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.447429,11.775219]}}]}', NULL, CAST(N'2023-02-10T00:00:00.0000000' AS DateTime2), 660, CAST(N'2023-01-20T00:00:00.0000000' AS DateTime2), 11, 6)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (115, N'Reforestación del Parque Ecológico Los Cedros', N'p0115', 6, 12, 2, 7, N'Reforestación y conservación del Parque Ecológico Los Cedros', N'Preservar el medio ambiente y la biodiversidad local', 1200000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Polygon","coordinates":[[[-72.433727,11.770326],[-72.432437,11.770453],[-72.432549,11.77141],[-72.43387,11.771201],[-72.433727,11.770326]]]}}]}', NULL, CAST(N'2023-02-15T00:00:00.0000000' AS DateTime2), 660, CAST(N'2023-02-01T00:00:00.0000000' AS DateTime2), 15, 7)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (116, N'Fomento al Empleo Juvenil', N'p0116', 7, 14, 2, 8, N'Programa para crear oportunidades de empleo para jóvenes en San Andrés', N'Reducir el desempleo juvenil y promover el emprendimiento', 2800000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.439136,11.775052]}}]}', NULL, CAST(N'2023-02-20T00:00:00.0000000' AS DateTime2), 660, CAST(N'2023-02-01T00:00:00.0000000' AS DateTime2), 8, 8)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (117, N'Mejora de Servicios de Policía en El Pinar', N'p0117', 8, 16, 2, 9, N'Mejora de infraestructura y servicios de policía en El Pinar', N'Aumentar la seguridad y confianza ciudadana', 1900000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"LineString","coordinates":[[-72.446562,11.778386],[-72.446315,11.776021],[-72.448483,11.775569],[-72.448997,11.777315]]}}]}', NULL, CAST(N'2023-03-05T00:00:00.0000000' AS DateTime2), 630, CAST(N'2023-02-15T00:00:00.0000000' AS DateTime2), 16, 9)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (118, N'Mejora de Sistemas Contables en la Alcaldía', N'p0118', 9, 18, 2, 10, N'Actualización y mejora de sistemas contables en la Alcaldía', N'Optimizar la gestión financiera del gobierno municipal', 1500000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.445237,11.773964]}}]}', NULL, CAST(N'2023-01-25T00:00:00.0000000' AS DateTime2), 690, CAST(N'2023-01-15T00:00:00.0000000' AS DateTime2), 17, 9)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (119, N'Capacitación en Presupuesto para Funcionarios', N'p0119', 9, 19, 2, 1, N'Expansión de programas de formación en presupuesto para funcionarios', N'Mejorar la gestión financiera y presupuestaria del municipio', 1700000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.446544,11.7781]}}]}', NULL, CAST(N'2023-01-20T00:00:00.0000000' AS DateTime2), 690, CAST(N'2023-01-10T00:00:00.0000000' AS DateTime2), 17, 9)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (120, N'Mejora del Equipamiento para Bomberos en Los Olivos', N'p0120', 8, 17, 2, 10, N'Adquisición de nuevo equipamiento para bomberos en Los Olivos', N'Fortalecer la capacidad operativa y de rescate', 2000000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.442056,11.777682]}}]}', NULL, CAST(N'2023-03-10T00:00:00.0000000' AS DateTime2), 630, CAST(N'2023-03-01T00:00:00.0000000' AS DateTime2), 16, 10)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (121, N'Construcción de Viviendas Sustentables en Valle Verde', N'p0121', 4, 9, 2, 7, N'Construcción de viviendas ecológicas en Valle Verde', N'Promover la vivienda digna y sustentable', 3000000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Polygon","coordinates":[[[-72.429042,11.759122],[-72.429557,11.757315],[-72.427819,11.756727],[-72.427225,11.758624],[-72.429042,11.759122]]]}}]}', NULL, CAST(N'2023-01-30T00:00:00.0000000' AS DateTime2), 690, CAST(N'2023-01-15T00:00:00.0000000' AS DateTime2), 7, 7)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (122, N'Implementación de Energías Renovables en Edificios Públicos', N'p0122', 6, 13, 2, 5, N'Instalación de sistemas de energía solar en edificios públicos', N'Fomentar el uso de energías limpias y renovables', 2200000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.444051,11.774634]}}]}', NULL, CAST(N'2023-02-25T00:00:00.0000000' AS DateTime2), 660, CAST(N'2023-02-01T00:00:00.0000000' AS DateTime2), 7, 5)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (123, N'Apoyo a Comercios Locales en San Felipe', N'p0123', 7, 15, 2, 8, N'Programa para impulsar el comercio local en San Felipe', N'Fortalecer la economía local y generar empleo', 1900000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.441581,11.775406]}}]}', NULL, CAST(N'2023-03-05T00:00:00.0000000' AS DateTime2), 630, CAST(N'2023-02-15T00:00:00.0000000' AS DateTime2), 9, 8)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (124, N'Renovación del Centro Cultural La Esperanza', N'p0124', 5, 11, 2, 6, N'Renovación del Centro Cultural La Esperanza', N'Preservar y promover la cultura local', 1400000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.444548,11.774314]}}]}', NULL, CAST(N'2023-01-25T00:00:00.0000000' AS DateTime2), 690, CAST(N'2023-01-15T00:00:00.0000000' AS DateTime2), 14, 6)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (125, N'Protección de Humedales en Laguna Verde', N'p0125', 6, 12, 2, 7, N'Conservación y protección de humedales en Laguna Verde', N'Preservar la biodiversidad y los recursos naturales', 2500000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Polygon","coordinates":[[[-72.459869,11.7661],[-72.45944,11.765007],[-72.45811,11.765301],[-72.458367,11.759373],[-72.455235,11.75849],[-72.45223,11.763578],[-72.456007,11.767025],[-72.459354,11.767109],[-72.459869,11.7661]]]}}]}', NULL, CAST(N'2023-02-20T00:00:00.0000000' AS DateTime2), 660, CAST(N'2023-02-01T00:00:00.0000000' AS DateTime2), 6, 7)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (126, N'Promoción de Eventos Culturales en Plaza Mayor', N'p0126', 5, 10, 2, 6, N'Organización de eventos culturales en la Plaza Mayor', N'Fomentar la participación cultural y el ocio saludable', 900000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.448357,11.7749]}}]}', NULL, CAST(N'2023-02-10T00:00:00.0000000' AS DateTime2), 660, CAST(N'2023-01-20T00:00:00.0000000' AS DateTime2), 11, 6)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (127, N'Conservación del Patrimonio Histórico en San Juan', N'p0127', 5, 11, 2, 6, N'Programa de conservación del patrimonio histórico en San Juan', N'Preservar el patrimonio cultural y arquitectónico', 1700000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Polygon","coordinates":[[[-72.439745,11.775314],[-72.439832,11.774562],[-72.438782,11.77467],[-72.438723,11.775513],[-72.439745,11.775314]]]}}]}', NULL, CAST(N'2023-02-15T00:00:00.0000000' AS DateTime2), 660, CAST(N'2023-02-01T00:00:00.0000000' AS DateTime2), 14, 6)
INSERT [dbo].[Project] ([Id], [Name], [Code], [Sector], [Subsector], [Stage], [Office], [Description], [Objectives], [PlannedCost], [Location], [ActualEndDate], [ActualStartDate], [PlannedDuration], [PlannedStartDate], [Sdg], [ExecutingAgency]) VALUES (128, N'Renovación del Centro de Salud Las Salinas', N'p0128', 2, 5, 2, 3, N'Mejora y modernización del Centro de Salud Las Salinas', N'Mejorar la accesibilidad y calidad de los servicios de salud', 2100000, N'{"type":"FeatureCollection","features":[{"type":"Feature","properties":{},"geometry":{"type":"Point","coordinates":[-72.442794,11.768851]}}]}', NULL, CAST(N'2023-01-30T00:00:00.0000000' AS DateTime2), 690, CAST(N'2023-01-15T00:00:00.0000000' AS DateTime2), 3, 3)
SET IDENTITY_INSERT [dbo].[Project] OFF
GO
SET IDENTITY_INSERT [dbo].[ProjectFunding] ON 

INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (1, 1, 1, 1, 400000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (2, 1, 2, 2, 100000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (3, 2, 3, 3, 200000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (4, 2, 2, 4, 1000000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (5, 3, 1, 1, 1500000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (6, 3, 2, 2, 500000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (7, 4, 3, 3, 500000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (8, 4, 1, 1, 300000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (9, 5, 2, 2, 100000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (10, 5, 1, 1, 200000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (11, 6, 4, 4, 500000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (12, 6, 1, 1, 500000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (13, 7, 1, 1, 1000000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (14, 7, 2, 2, 500000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (15, 8, 3, 3, 300000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (16, 8, 1, 1, 400000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (17, 9, 1, 1, 300000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (18, 9, 2, 2, 200000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (19, 10, 4, 4, 200000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (20, 10, 1, 1, 200000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (113, 100, 2, 3, 1500000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (114, 101, 1, 1, 700000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (115, 101, 2, 4, 1400000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (116, 102, 3, 5, 1300000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (117, 103, 1, 1, 800000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (118, 103, 3, 7, 2000000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (119, 104, 2, 4, 1750000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (120, 104, 3, 6, 1750000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (121, 105, 1, 1, 500000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (122, 105, 3, 5, 400000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (123, 106, 2, 3, 900000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (124, 107, 1, 1, 1100000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (125, 107, 2, 2, 1100000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (126, 108, 3, 9, 2900000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (127, 109, 1, 1, 600000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (128, 109, 2, 2, 600000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (129, 110, 2, 3, 2500000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (130, 111, 1, 1, 900000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (131, 111, 3, 5, 900000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (132, 112, 2, 4, 2500000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (133, 113, 1, 1, 1200000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (134, 113, 3, 7, 900000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (135, 114, 3, 5, 800000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (136, 115, 1, 1, 600000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (137, 115, 2, 2, 600000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (138, 116, 1, 1, 700000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (139, 116, 2, 4, 2100000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (140, 117, 1, 1, 950000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (141, 117, 3, 9, 950000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (142, 118, 3, 6, 1500000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (143, 119, 1, 1, 800000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (144, 119, 3, 5, 900000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (145, 120, 1, 1, 1000000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (146, 120, 2, 2, 1000000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (147, 121, 2, 4, 1500000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (148, 121, 3, 6, 1500000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (149, 122, 3, 7, 2200000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (150, 123, 1, 1, 950000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (151, 123, 2, 2, 950000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (152, 124, 1, 1, 700000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (153, 124, 3, 5, 700000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (154, 125, 2, 4, 1250000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (155, 125, 3, 6, 1250000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (156, 126, 3, 5, 900000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (157, 127, 1, 1, 850000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (158, 127, 2, 2, 850000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (159, 128, 1, 1, 700000)
INSERT [dbo].[ProjectFunding] ([Id], [Project], [Type], [Source], [Value]) VALUES (160, 128, 3, 5, 1400000)
SET IDENTITY_INSERT [dbo].[ProjectFunding] OFF
GO
SET IDENTITY_INSERT [dbo].[ProjectStage] ON 

INSERT [dbo].[ProjectStage] ([Id], [Name], [Order]) VALUES (1, N'Planeación', 1)
INSERT [dbo].[ProjectStage] ([Id], [Name], [Order]) VALUES (2, N'En ejecución', 2)
INSERT [dbo].[ProjectStage] ([Id], [Name], [Order]) VALUES (3, N'Terminado', 3)
SET IDENTITY_INSERT [dbo].[ProjectStage] OFF
GO
SET IDENTITY_INSERT [dbo].[Sdg] ON 

INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (1, 1, N'Fin de la pobreza')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (2, 2, N'Hambre cero')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (3, 3, N'Salud y bienestar')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (4, 4, N'Educación de calidad')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (5, 5, N'Igualdad de género')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (6, 6, N'Agua limpia y saneamiento')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (7, 7, N'Energía asequible y no contaminante')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (8, 8, N'Trabajo decente y crecimiento económico')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (9, 9, N'Industria, innovación e infraestructura')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (10, 10, N'Reducción de las desigualdades')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (11, 11, N'Ciudades y comunidades sostenibles')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (12, 12, N'Consumo y producción responsables')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (13, 13, N'Acción por el clima')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (14, 14, N'Vida submarina')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (15, 15, N'Vida de ecosistemas terrestres')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (16, 16, N'Paz, justicia e instituciones sólidas')
INSERT [dbo].[Sdg] ([Id], [Number], [Title]) VALUES (17, 17, N'Alianzas para lograr los objetivos')
SET IDENTITY_INSERT [dbo].[Sdg] OFF
GO
SET IDENTITY_INSERT [dbo].[Sector] ON 

INSERT [dbo].[Sector] ([Id], [Name]) VALUES (1, N'Educación')
INSERT [dbo].[Sector] ([Id], [Name]) VALUES (2, N'Salud')
INSERT [dbo].[Sector] ([Id], [Name]) VALUES (3, N'Infraestructura')
INSERT [dbo].[Sector] ([Id], [Name]) VALUES (4, N'Desarrollo Social')
INSERT [dbo].[Sector] ([Id], [Name]) VALUES (5, N'Cultura')
INSERT [dbo].[Sector] ([Id], [Name]) VALUES (6, N'Medio Ambiente')
INSERT [dbo].[Sector] ([Id], [Name]) VALUES (7, N'Economía')
INSERT [dbo].[Sector] ([Id], [Name]) VALUES (8, N'Seguridad Pública')
INSERT [dbo].[Sector] ([Id], [Name]) VALUES (9, N'Finanzas')
SET IDENTITY_INSERT [dbo].[Sector] OFF
GO
SET IDENTITY_INSERT [dbo].[Subsector] ON 

INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (1, 1, N'Primaria')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (2, 1, N'Secundaria')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (3, 1, N'Universitaria')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (4, 2, N'Hospitales')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (5, 2, N'Centros de Salud')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (6, 3, N'Carreteras')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (7, 3, N'Edificios Públicos')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (8, 4, N'Programas Sociales')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (9, 4, N'Vivienda')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (10, 5, N'Eventos Culturales')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (11, 5, N'Patrimonio')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (12, 6, N'Conservación')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (13, 6, N'Energías Renovables')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (14, 7, N'Empleo')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (15, 7, N'Comercio')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (16, 8, N'Policía')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (17, 8, N'Bomberos')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (18, 9, N'Contabilidad')
INSERT [dbo].[Subsector] ([Id], [Sector], [Name]) VALUES (19, 9, N'Presupuesto')
SET IDENTITY_INSERT [dbo].[Subsector] OFF
GO
SET IDENTITY_INSERT [dbo].[TaskStage] ON 

INSERT [dbo].[TaskStage] ([Id], [Name], [Order]) VALUES (1, N'Presentada', 1)
INSERT [dbo].[TaskStage] ([Id], [Name], [Order]) VALUES (2, N'Aprobada', 2)
SET IDENTITY_INSERT [dbo].[TaskStage] OFF
GO
SET IDENTITY_INSERT [dbo].[UserProfile] ON 

INSERT [dbo].[UserProfile] ([Id], [AspNetUserId], [Email], [Name], [Surname], [Office], [Notes]) VALUES (1, N'481437b0-2c9c-4267-a91e-a7ffe6290224', N'admin@mail.com', N'System', N'Admin', NULL, NULL)
INSERT [dbo].[UserProfile] ([Id], [AspNetUserId], [Email], [Name], [Surname], [Office], [Notes]) VALUES (2, N'8a7da9ee-f64a-45af-b06e-ccd0196fa9c6', N'direccion@mail.com', N'Direccion', N'Direccion', NULL, NULL)
INSERT [dbo].[UserProfile] ([Id], [AspNetUserId], [Email], [Name], [Surname], [Office], [Notes]) VALUES (3, N'824c4b6f-473f-4c8e-849f-1667407dc10c', N'operacion@mail.com', N'Jaime', N'Osorio', NULL, N'Usuario para pruebas de Rol Operación')
SET IDENTITY_INSERT [dbo].[UserProfile] OFF
GO
ALTER TABLE [dbo].[ProjectStage] ADD  DEFAULT ((0)) FOR [Order]
GO
ALTER TABLE [dbo].[Addition]  WITH CHECK ADD  CONSTRAINT [FK_Addition_Product_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Product] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Addition] CHECK CONSTRAINT [FK_Addition_Product_Product]
GO
ALTER TABLE [dbo].[Addition]  WITH CHECK ADD  CONSTRAINT [FK_Addition_TaskStage_Stage] FOREIGN KEY([Stage])
REFERENCES [dbo].[TaskStage] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Addition] CHECK CONSTRAINT [FK_Addition_TaskStage_Stage]
GO
ALTER TABLE [dbo].[AdditionAttachment]  WITH CHECK ADD  CONSTRAINT [FK_AdditionAttachment_Addition_Addition] FOREIGN KEY([Addition])
REFERENCES [dbo].[Addition] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdditionAttachment] CHECK CONSTRAINT [FK_AdditionAttachment_Addition_Addition]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[City]  WITH CHECK ADD  CONSTRAINT [FK_City_Province_Province] FOREIGN KEY([Province])
REFERENCES [dbo].[Province] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[City] CHECK CONSTRAINT [FK_City_Province_Province]
GO
ALTER TABLE [dbo].[Extension]  WITH CHECK ADD  CONSTRAINT [FK_Extension_Project_Project] FOREIGN KEY([Project])
REFERENCES [dbo].[Project] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Extension] CHECK CONSTRAINT [FK_Extension_Project_Project]
GO
ALTER TABLE [dbo].[Extension]  WITH CHECK ADD  CONSTRAINT [FK_Extension_TaskStage_Stage] FOREIGN KEY([Stage])
REFERENCES [dbo].[TaskStage] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Extension] CHECK CONSTRAINT [FK_Extension_TaskStage_Stage]
GO
ALTER TABLE [dbo].[ExtensionAttachment]  WITH CHECK ADD  CONSTRAINT [FK_ExtensionAttachment_Extension_Extension] FOREIGN KEY([Extension])
REFERENCES [dbo].[Extension] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ExtensionAttachment] CHECK CONSTRAINT [FK_ExtensionAttachment_Extension_Extension]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_PaymentStage_Stage] FOREIGN KEY([Stage])
REFERENCES [dbo].[PaymentStage] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_PaymentStage_Stage]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_Product_Product] FOREIGN KEY([Product])
REFERENCES [dbo].[Product] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_Product_Product]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_Payment_ProjectFunding_FundingSource] FOREIGN KEY([FundingSource])
REFERENCES [dbo].[ProjectFunding] ([Id])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_Payment_ProjectFunding_FundingSource]
GO
ALTER TABLE [dbo].[PaymentAttachment]  WITH CHECK ADD  CONSTRAINT [FK_PaymentAttachment_Payment_Payment] FOREIGN KEY([Payment])
REFERENCES [dbo].[Payment] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PaymentAttachment] CHECK CONSTRAINT [FK_PaymentAttachment_Payment_Payment]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Project_Project] FOREIGN KEY([Project])
REFERENCES [dbo].[Project] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Project_Project]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Agency_ExecutingAgency] FOREIGN KEY([ExecutingAgency])
REFERENCES [dbo].[Agency] ([Id])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Agency_ExecutingAgency]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Office_Office] FOREIGN KEY([Office])
REFERENCES [dbo].[Office] ([Id])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Office_Office]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_ProjectStage_Stage] FOREIGN KEY([Stage])
REFERENCES [dbo].[ProjectStage] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_ProjectStage_Stage]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Sdg_Sdg] FOREIGN KEY([Sdg])
REFERENCES [dbo].[Sdg] ([Id])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Sdg_Sdg]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Sector_Sector] FOREIGN KEY([Sector])
REFERENCES [dbo].[Sector] ([Id])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Sector_Sector]
GO
ALTER TABLE [dbo].[Project]  WITH CHECK ADD  CONSTRAINT [FK_Project_Subsector_Subsector] FOREIGN KEY([Subsector])
REFERENCES [dbo].[Subsector] ([Id])
GO
ALTER TABLE [dbo].[Project] CHECK CONSTRAINT [FK_Project_Subsector_Subsector]
GO
ALTER TABLE [dbo].[ProjectAttachment]  WITH CHECK ADD  CONSTRAINT [FK_ProjectAttachment_Project_Project] FOREIGN KEY([Project])
REFERENCES [dbo].[Project] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectAttachment] CHECK CONSTRAINT [FK_ProjectAttachment_Project_Project]
GO
ALTER TABLE [dbo].[ProjectFunding]  WITH CHECK ADD  CONSTRAINT [FK_ProjectFunding_FundingAgency_Source] FOREIGN KEY([Source])
REFERENCES [dbo].[FundingAgency] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectFunding] CHECK CONSTRAINT [FK_ProjectFunding_FundingAgency_Source]
GO
ALTER TABLE [dbo].[ProjectFunding]  WITH CHECK ADD  CONSTRAINT [FK_ProjectFunding_FundingType_Type] FOREIGN KEY([Type])
REFERENCES [dbo].[FundingType] ([Id])
GO
ALTER TABLE [dbo].[ProjectFunding] CHECK CONSTRAINT [FK_ProjectFunding_FundingType_Type]
GO
ALTER TABLE [dbo].[ProjectFunding]  WITH CHECK ADD  CONSTRAINT [FK_ProjectFunding_Project_Project] FOREIGN KEY([Project])
REFERENCES [dbo].[Project] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectFunding] CHECK CONSTRAINT [FK_ProjectFunding_Project_Project]
GO
ALTER TABLE [dbo].[ProjectImage]  WITH CHECK ADD  CONSTRAINT [FK_ProjectImage_Project_Project] FOREIGN KEY([Project])
REFERENCES [dbo].[Project] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectImage] CHECK CONSTRAINT [FK_ProjectImage_Project_Project]
GO
ALTER TABLE [dbo].[ProjectVideo]  WITH CHECK ADD  CONSTRAINT [FK_ProjectVideo_Project_Project] FOREIGN KEY([Project])
REFERENCES [dbo].[Project] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjectVideo] CHECK CONSTRAINT [FK_ProjectVideo_Project_Project]
GO
ALTER TABLE [dbo].[Subsector]  WITH CHECK ADD  CONSTRAINT [FK_Subsector_Sector_Sector] FOREIGN KEY([Sector])
REFERENCES [dbo].[Sector] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Subsector] CHECK CONSTRAINT [FK_Subsector_Sector_Sector]
GO
ALTER TABLE [dbo].[UserProfile]  WITH CHECK ADD  CONSTRAINT [FK_UserProfile_Office_Office] FOREIGN KEY([Office])
REFERENCES [dbo].[Office] ([Id])
GO
ALTER TABLE [dbo].[UserProfile] CHECK CONSTRAINT [FK_UserProfile_Office_Office]
GO
USE [master]
GO
ALTER DATABASE [IMRepo_Synthetic] SET  READ_WRITE 
GO
