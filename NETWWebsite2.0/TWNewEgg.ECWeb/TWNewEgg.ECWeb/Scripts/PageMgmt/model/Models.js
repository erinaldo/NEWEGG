angular.module('DB', [])
    .value('Slider', {})
    .value('Resource', {
        imageServerPath: 'https://negcdn.azureedge.net/www',
        subpath: '/Pic/PageMgmt/',
        CompStatus: { SAVED: "S", EDIT: "E", NEW: "N", DELETE: "D" },
        PageStatus: { Editing: "E", Waiting: "W", Reject: "R", Active: "A", Deactive: "D" }
    })
    .value('PageData', {
        PageInfo: {},
        Pages: [],
        ctrl: {}
    })
    .value('ApiRegist', [
        {
            name: 'Type1',
            apis: [
            { name: '賣場', params: ['id'], api: '/PMItem/Type1', templateUrl: '/Content/PageMgmt/template/Object/Type1.html' }
            ]
        },
        {
            name: 'Type2',
            apis: [
            { name: '賣場', params: ['id'], api: '/PMItem/Type1', templateUrl: '/Content/PageMgmt/template/Object/Type2.html' }
            ]
        },
        {
            name: 'Type3',
            apis: [
            { name: '賣場', params: ['id'], api: '/PMItem/Type1', templateUrl: '/Content/PageMgmt/template/Object/Type3.html' }
            ]
        },
        {
            name: 'Type4',
            apis: [
            { name: '賣場', params: ['id'], api: '/PMItem/Type1', templateUrl: '/Content/PageMgmt/template/Object/Type4.html' }
            ]
        },
        {
            name: 'Type5',
            apis: [
            { name: '賣場', params: ['id'], api: '/PMItem/Type1', templateUrl: '/Content/PageMgmt/template/Object/Type5.html' }
            ]
        }
    ])
    .factory('Model', function () {
        var ctrl = function () {
            this.isGrid = false;
            this.isObjEdit = false;
            this.onEditComp = {};
            this.isObjEditable = true;
        }
        var PageInfo = function (data) {
            this.PageID = data.PageID || -1;
            this.Name = data.Name || '';
            this.Description = data.Description || '';
            this.Path = data.Path || '';
            this.ParentID = data.ParentID || 0;
            this.PageOrder = data.PageOrder || 0;
            this.Status = data.Status || 'E';
            this.Height = data.Height || 800;
            this.Width = data.Width || 1000;
            this.BackgroundImg = data.BackgroundImg || null;
            this.InUser = data.InUser || '';
            this.InDate = data.InDate || new Date();
            this.LastEditUser = data.LastEditUser || '';
            this.LastEditDate = data.LastEditDate || null;

            this.ComponentInfo = [];
            this.ext = {
                selectedComp:{},
                pickedComps:[],
                isEditPage: false
            }
        }
        var ComponentInfo = function (data) {
            this.PageID = data.PageID;
            this.ObjectID = data.ObjectID;
            this.ObjectType = data.ObjectType;
            this.Index = data.Index;
            this.ComponentID = data.ComponentID || -1;
            this.Height = data.Height || 100;
            this.HitCount = data.HitCount || 0;
            this.InDate = data.InDate || "";
            this.InUser = data.InUser || "";
            this.LastEditDate = data.LastEditDate || "";
            this.LastEditUser = data.LastEditUser || "";
            this.Status = data.Status || "N";
            this.Width = data.Width || 100;
            this.XIndex = data.XIndex || 0;
            this.YIndex = data.YIndex || $(document).scrollTop();
            this.ZIndex = data.ZIndex || 500;

            this.Image = [];
            this.Text = {};
            this.Video = {};
            this.Dynamic = {};
            this.CustomizedObject = {};
            this.ext = {
                picked: false,
                hover: false
            };
            this.extf = {

            }
        }
        var Text = function (data) {
            this.Content = data.Content || "文字物件";
            this.HitCount = data.HitCount || 0;
            this.InDate = data.InDate || "";
            this.InUser = data.InUser || "";
            this.LastEditDate = data.LastEditDate || null;
            this.LastEditUser = data.LastEditUser || null;
            this.TextID = data.TextID;
        }
        var Image = function (data) {
            this.ImageID = data.ImageID;
            this.AlbumImageID = data.AlbumImageID || 0;
            this.Path = data.Path || "/PageMgmt/Default.jpg";
            this.Target = data.Target || "_blank";
            this.Speed = data.Speed || 3;
            this.FileName = data.FileName || "Default.jpg";
            this.Effect = data.Effect || "N";
            this.Title = data.Title || "Default.jpg";
            this.Description = data.Description || "";
            this.EffectGroupID = data.EffectGroupID || this.ImageID;
            this.EffectGroupOrder = data.EffectGroupOrder || 1;
            this.Hyperlink = data.Hyperlink || "";
            this.InUser = data.InUser || "";
            this.InDate = data.InDate || "";
            this.LastEditDate = data.LastEditDate || null;
            this.LastEditUser = data.LastEditUser || null;
        }
        var Video = function (data) {
            this.VideoID = data.VideoID;
            this.DarenID = data.DarenID || 0;
            this.ProviderVideoID = data.ProviderVideoID || "Default";
            this.VideoCategoryID = data.VideoCategoryID || -1;
            this.Status = data.Status || 0;
            this.Duration = data.Duration || "00:00";
            this.ThumbnailUrl = data.ThumbnailUrl || "/PageMgmt/youkulogo.jpg";
            this.Title = data.Title || "影片物件";
            this.Description = data.Description || "";
            this.ProviderID = angular.isNumber(data.ProviderID) ? data.ProviderID : 1;
            this.ChannelTitle = data.ChannelTitle || "";
            this.ViewCount = data.ViewCount || 0;
            this.PublishedAt = data.PublishedAt || (new Date).getDate();
            this.InUser = data.InUser || "";
            this.InDate = data.InDate || "";
            this.LastEditDate = data.LastEditDate || null;
            this.LastEditUser = data.LastEditUser || null;
        }
        var Dynamic = function (data) {
            this.DynamicID = data.DynamicID;
            this.Type = data.Type || 'Type1';
            this.Api = data.Api || '/PMItem/Type1';
            this.Parameters = data.Parameters || '/1';
            this.TemplateUrl = '';
        }
        return {
            p: PageInfo,
            c: ComponentInfo,
            Text: Text,
            Image: Image,
            Video: Video,
            Dynamic: Dynamic,
            ctrl: ctrl
        }
    })
.factory('tinymceConfig', function () {
    var config = function () {
        this.menubar = false;
        this.selector = "";
        this.theme = "modern";
        this.language = 'zh_CN';
        this.plugins = [
             "advlist autolink link lists charmap hr anchor pagebreak spellchecker",
             "searchreplace visualblocks visualchars code fullscreen insertdatetime media nonbreaking",
             "save table contextmenu directionality emoticons template paste textcolor"
        ];
        this.toolbar = "undo redo | formatselect fontselect fontsizeselect | forecolor backcolor | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link | myimgbutton media";
        this.toolbar_items_size = 'small';
        this.content_css = "/Themes/2013/stylesheets/MyTinyMCE.css";
        this.formats = {
            forecolor: { inline: 'span', styles: { color: '%value' } }
        };
        this.theme_advanced_font_sizes = "10px,12px,13px,14px,16px,18px,20px";
        this.theme_advanced_blockformats = "h1,h2,h3,p";
        this.theme_modern_fonts = "Andale Mono=andale mono,times;" +
                    "Arial=arial,helvetica,sans-serif;" +
                    "Arial Black=arial black,avant garde;" +
                    "Book Antiqua=book_antiquaregular,palatino;" +
                    "Corda Light=CordaLight,sans-serif;" +
                    "Courier New=courier_newregular,courier;" +
                    "Flexo Caps=FlexoCapsDEMORegular;" +
                    "Lucida Console=lucida_consoleregular,courier;" +
                    "Georgia=georgia,palatino;" +
                    "Helvetica=helvetica;" +
                    "Impact=impactregular,chicago;" +
                    "Museo Slab=MuseoSlab500Regular,sans-serif;" +
                    "Museo Sans=MuseoSans500Regular,sans-serif;" +
                    "Oblik Bold=OblikBoldRegular;" +
                    "Sofia Pro Light=SofiaProLightRegular;" +
                    "Symbol=webfontregular;" +
                    "Tahoma=tahoma,arial,helvetica,sans-serif;" +
                    "Terminal=terminal,monaco;" +
                    "Tikal Sans Medium=TikalSansMediumMedium;" +
                    "Times New Roman=times new roman,times;" +
                    "Trebuchet MS=trebuchet ms,geneva;" +
                    "Verdana=verdana,geneva;" +
                    "Webdings=webdings;" +
                    "Wingdings=wingdings,zapf dingbats" +
                    "Aclonica=Aclonica, sans-serif;" +
                    "Michroma=Michroma;" +
                    "Paytone One=Paytone One, sans-serif;" +
                    "Andalus=andalusregular, sans-serif;" +
                    "Arabic Style=b_arabic_styleregular, sans-serif;" +
                    "Andalus=andalusregular, sans-serif;" +
                    "KACST_1=kacstoneregular, sans-serif;" +
                    "Mothanna=mothannaregular, sans-serif;" +
                    "Nastaliq=irannastaliqregular, sans-serif;" +
                    "Samman=sammanregular";
        this.font_size_style_values = "12px,13px,14px,16px,18px,20px";
        this.height = '300';
        this.entity_encoding = "raw";
        this.body_class = 'MyTinyMCE';
        this.setup = function () { };
    };
    return config;
});
    