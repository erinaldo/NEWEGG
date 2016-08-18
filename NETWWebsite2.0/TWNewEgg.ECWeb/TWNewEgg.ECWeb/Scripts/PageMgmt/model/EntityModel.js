angular.module('EntityModel', ['VideoServices'])
    .factory('entityModel', function (videoAPIs) {
        var BlogArticle = function (article) {
            this.AccountName = article.AccountName;
            this.ArticleHref = article.ArticleHref;
            this.CategoryID = article.CategoryID;
            this.Content = article.Content;
            this.CoverImageURL = article.CoverImageURL;
            this.InDate = article.InDate;
            this.LikeCount = article.LikeCount;
            this.Title = article.Title;
            this.VisitToday = article.VisitToday;
            this.VisitTotal = article.VisitTotal;
            this.contentText = article.Content;
            this.DarenCategoryName = article.DarenCategoryName;
        };

        var VideoInfo = function (video) {
            this.VideoID = video.VideoID
            this.ThumbnailUrl = video.ThumbnailUrl;
            this.DarenID = video.DarenID;
            this.Description = video.Description;
            this.Duration = video.Duration;
            this.InDate = video.InDate;
            this.Title = video.Title;
            this.VideoCategory = video.VideoCategory;
            this.ViewCount = video.ViewCount;
            this.ProviderID = video.ProviderID;
            this.ProviderVideoID = video.ProviderVideoID;
        };
        angular.extend(VideoInfo.prototype, (function () {
            return {
                openVideo: function () {
                    videoAPIs.openVideo(this);
                }
            };
        })());

        var AccountInfo = function (account) {
            this.Nickname = account.Nickname;
            this.AccountName = account.AccountName;
            this.HeadPortraitURL = account.HeadPortraitURL;
            this.DarenIndex = '/Blog/DarenBlog/' + account.AccountName;
        };

        var DarenInfo = function (daren) {
            this.AccountName = daren.AccountName;
            this.DarenID = daren.DarenID;
            this.DarenIntro = daren.DarenIntro;
            this.DarenTitle = daren.DarenTitle;
            this.DarenType = daren.DarenType;
            this.FansCountToday = daren.FansCountToday;
            this.FansCountTotal = daren.FansCountTotal;
            this.FansCountWeek = daren.FansCountWeek;
            this.Nickname = daren.Nickname;
            this.LikeCount = daren.LikeCount;
            this.DarenCategoryName = daren.DarenCategoryName;
        };

        var ReportInfo = function (report) {
            this.ReportOrder = report.ReportOrder;
            this.FeatureValue = report.FeatureValue;
            this.ReportInfoID = report.ReportInfoID;
            this.RelationalID = report.RelationalID;
            this.ReportConditionID = report.ReportConditionID;
            if (!report.condition) {
                this.condition = {
                    OrderBy: report.OrderBy,
                    Period: report.Period,
                    DataType: report.DataType,
                    Keywords: report.Condition,
                    AccountName: report.AccountName,
                    CategoryID: report.CategoryID
                };            
            } else {
                this.condition = report.condition;
            }
        };

        var Article = function (article) {
            this.ID = article.ID;
            this.MainTagID = article.MainTagID;
            this.MainTagName = article.MainTagName;
            this.CollectionType = article.CollectionType;
            this.Title = article.Title;
            this.ShortTitle =article.ShortTitle;
            this.Content = article.Content;
            this.Brief = article.Brief;
            this.Duration = article.Duration;
            this.IsLike = article.IsLike;
            this.LikeCount = article.LikeCount;
            this.IconUrl = article.IconUrl;
            this.ImgUrl = article.ImgUrl;
            this.Status = article.Status;
            this.OtherInfo = article.OtherInfo;
            this.InDate = article.InDate;
            this.tags = article.tags;
            this.routeInfo = article.routeInfo;
            this.ArticleHref = article.ArticleHref;
        };

        return {
            B: BlogArticle,
            V: VideoInfo,
            A: AccountInfo,
            D: DarenInfo,
            R: ReportInfo,
            T: Article,
            DCItem: Article,
            News: Article
        };
    });