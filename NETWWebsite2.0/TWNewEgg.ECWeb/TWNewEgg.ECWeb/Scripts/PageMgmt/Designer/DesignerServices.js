angular.module('DesignerServices', ['Status', 'DB'])
    .service('Resources', function ($http) {
        var pagemgmtResx = {};
        var propertyResx = {};
        var sitemapResx = {};
        var cusObjResx = {};

        $http.post('/Designer/GetResx').success(function (data) {
            if (data.pagemgmtResx) {
                for (var i in data.pagemgmtResx) {
                    pagemgmtResx[data.pagemgmtResx[i].Key] = data.pagemgmtResx[i].Value;
                }
            }
            if (data.propertyResx) {
                for (var i in data.propertyResx) {
                    propertyResx[data.propertyResx[i].Key] = data.propertyResx[i].Value;
                }
            }
            if (data.sitemapResx) {
                for (var i in data.sitemapResx) {
                    sitemapResx[data.sitemapResx[i].Key] = data.sitemapResx[i].Value;
                }
            }
            if (data.cusObjResx) {
                for (var prop in data.cusObjResx) {
                    cusObjResx[data.cusObjResx[prop].Key] = data.cusObjResx[prop].Value;
                }
            }
        });

        this.getPropertyResx = function () {
            return propertyResx;
        };
        this.getPageMgmtResx = function () {
            return pagemgmtResx;
        };
        this.getSiteMapResx = function () {
            return sitemapResx;
        };
        this.getCusObjMapResx = function () {
            return cusObjResx;
        };
    })
    .service('controlService', function ($rootScope, Status, PageData) {
        var con = {};
        con.selectComponent = function (component, e) {
            PageData.PageInfo.ext.selectedComp = component;
            if (e.ctrlKey) {
                if (!component.ext.picked) {
                    component.ext.picked = true;
                    PageData.PageInfo.ext.pickedComps.push(component);
                } else {
                    component.ext.picked = false;
                    var index = PageData.PageInfo.ext.pickedComps.indexOf(component);
                    if (index > -1) {
                        PageData.PageInfo.ext.pickedComps.splice(index, 1);
                    }
                }
            }
            if (component.Status == Status.ComponentStatus.SAVED) { //上線
                component.Status = Status.ComponentStatus.EDIT; //編輯中
            }
        }
        
        con.selectAll = function () {
            for (var i in PageData.PageInfo.ComponentInfo) {
                var comp = PageData.PageInfo.ComponentInfo[i];
                comp.ext.picked = true;
                PageData.PageInfo.ext.pickedComps.push(comp);
            }
        }
        con.multiDrag = function (ComponentID, moveX, moveY) {
            for (var i in PageData.PageInfo.ext.pickedComps) {
                var comp = PageData.PageInfo.ext.pickedComps[i];
                comp.Status = Status.ComponentStatus.EDIT; //編輯中
                if (comp.ComponentID != ComponentID) {
                    comp.YIndex = parseInt(comp.YIndex) + moveY;
                    comp.XIndex = parseInt(comp.XIndex) + moveX;
                }
            }
        }

        con.cancelPicked = function () {
            for (var i in PageData.PageInfo.ext.pickedComps) {
                var comp = PageData.PageInfo.ext.pickedComps[i];
                comp.ext.picked = false;
            }
            PageData.PageInfo.ext.pickedComps.length = 0;
        }
        con.showGrid = function () {
            PageData.ctrl.isGrid = true;
        }
        con.hideGrid = function () {
            PageData.ctrl.isGrid = false;
        }
        return con;
    })
    .service('COPY', function ($rootScope, PageData, Status, DataService) {
        var components = [];
        var pasteCnt = 1;
        this.copyPicked = function () {
            components.length = 0;
            components = PageData.PageInfo.ext.pickedComps;
        }
        this.paste = function () {
            for (i in components) {
                var comp = {};
                var newid = DataService.newCompID();
                if (components[i].ObjectType === 'CustomizedObject') {
                    components[i].CustomizedObject.breakParentRelation();
                    angular.copy(components[i], comp);
                    comp.CustomizedObject.repairParentRelation();
                    components[i].CustomizedObject.repairParentRelation();
                } else {
                    angular.copy( components[i], comp);
                }
                    comp.ComponentID = newid;
                    comp.ext.picked = false;
                    comp.Status = Status.ComponentStatus['NEW'];
                    comp.PageID = PageData.PageInfo.PageID;
                comp.XIndex = comp.XIndex + 10 * pasteCnt;
                comp.YIndex = comp.YIndex + 10 * pasteCnt;
                    PageData.PageInfo.ComponentInfo[newid] = comp;
                pasteCnt += 1;
            }
        }
        this.init = function () { pasteCnt = 1; components.length = 0; }
        this.getComps = function () { return components; }
    })
    .service('DataService', function (Model, PageData, $filter, $rootScope, $http, objEditService, controlService, Slider, Resource) {
        this.getSiteMap = function (callback) {
            PageData.Pages.length = 0;
            $http.post('/designer/getsitemap/').success(function (data) {
                PageData.Pages = angular.copy(data);
                if (callback) { callback(); }
            });
        }
        this.selectPage = function (page, isEditPage, callback) {
            var load = this.load;
            this.init();
            PageData.PageInfo = new Model['p'](page);
            $http.post('/designer/editpage/'+page.Path+'/'+isEditPage)
                .success(function (response) {
                    var data = response.data;
                    var msg = response.msg;
                    if (msg == 'success') {
                        load({
                            PageInfo: data.page,
                            Text: data.texts,
                            Video: data.videos,
                            Image: data.images,
                            Dynamic: data.dynamics,
                            ComponentInfo: data.editComponents,
                            CustomizedObject: data.customizedObjs
                        });
                        PageData.PageInfo.Status = Resource.PageStatus['Editing'];
                        PageData.PageInfo['ext']['isEditPage'] = isEditPage;
                        if (callback) { callback(); }
                    } else if (msg == 'unavailable') {
                        window.location.href = "/Account/Login";
                    }
                });
        }
        this.getActivePage = function (Path, callback) {
            var load = this.load;
            PageData.PageInfo = new Model['p']({ Path: Path });
            PageData.PageInfo['ext']['isEditPage'] = false;
            $http.post('/designer/getactivepage?path='+Path).success(function (data) {
                load({
                    PageInfo: data.page,
                    Text: data.texts,
                    Video: data.videos,
                    Image: data.images,
                    Dynamic: data.dynamics,
                    ComponentInfo: data.editComponents,
                    CustomizedObject: data.customizedObjs
                });
                if (callback) callback(data);
            });
        }
        this.init = function () {
            $(document).scrollTop(0);
            PageData.PageInfo = new Model['p']({});
            PageData.ctrl = new Model['ctrl']();
            objEditService.close();
            for (var member in Slider) delete Slider[member];
        }
        this.savePage = function (callback) {
            var PageInfo = {};
            angular.extend(PageInfo, PageData.PageInfo);
            PageInfo.ComponentInfo = KeypairToArray(PageInfo.ComponentInfo);
            this.removeEmpty(PageInfo);
            $http.post('/Designer/SavePage/', { 'page': PageInfo }).success(function (data) {
                if (callback) callback(data, true);
            });
        };
        this.auditPage = function (callback) {
            var PageInfo = this.getOnlyPageInfo(PageData.PageInfo);
            $http.post('/Designer/AuditPage/', { 'page': PageInfo }).success(function (data) {
                if (callback) callback(data, true);
            });
        }
        this.rejectPage = function (callback) {
            var PageInfo = this.getOnlyPageInfo(PageData.PageInfo);
            $http.post('/Designer/Reject/', { 'page': PageInfo }).success(function (data) {
                if (callback) callback(data, true);
            });
        }
        this.launchPage = function (callback) {
            var PageInfo = this.getOnlyPageInfo(PageData.PageInfo);
            $http.post('/Designer/LaunchPage/', { 'page': PageInfo }).success(function (data) {
                if (callback) callback(data, false);
            });
        }
        this.recoverPage = function (callback) {
            var PageInfo = this.getOnlyPageInfo(PageData.PageInfo);
            $http.post('/Designer/CancelEdit/', { 'page': PageInfo }).success(function (data) {
                if (callback) callback(data, false);
            });
        }
        this.deletePage = function (page, callback) {
            $http.post('/designer/deletepage/', { page: page }).success(function (data) {
                if (callback) callback(data);
            });
        }
        this.addPage = function (page, callback) {
            $http.post('/designer/addpage/', { page: page }).success(function (data) {
                if (callback) callback(data, true);
            });
        }
        this.getOnlyPageInfo = function (page) {
            return new Model.p(page);
        }
        this.removeEmpty = function (obj) {
            for (var i in obj) {
                if (Object.prototype.toString.call(obj[i]) === '[object Array]') {
                    for (var j = 0; j < obj[i].length; j++) {
                        this.removeEmpty(obj[i][j]);
                    }
                } else {
                    if (obj[i] === null || obj[i] === undefined || obj[i] === "" || i === 'extf' || i === 'ext') {
                        delete obj[i];
                    }
                }
            }
        }
        this.load = function (data) {
            var temp = { ComponentInfo: {}, Image: [], Text: {}, Video: {}, Dynamic:{}, CustomizedObject: {} };
            for (i in data.Image) {
                var image = new Model['Image'](data.Image[i]);
                temp['Image'].push(image);
            }
            for (i in data.Text) {
                var text = new Model['Text'](data.Text[i]);
                temp['Text'][text.TextID] = text;
            }
            for (i in data.Video) {
                var video = new Model['Video'](data.Video[i]);
                temp['Video'][video.VideoID] = video;
            }
            for (i in data.Dynamic) {
                var dynamic = new Model['Dynamic'](data.Dynamic[i]);
                temp['Dynamic'][dynamic.DynamicID] = dynamic;
            }
            for (c in data.ComponentInfo) {
                var comp = new Model['c'](data.ComponentInfo[c]);
                if (comp.ObjectType === 'Image') {
                    comp['Image'] = $filter('filter')(temp['Image'], { EffectGroupID: comp.ObjectID }) || [];
                }
                else if (comp.ObjectType === 'Text') {
                    comp['Text'] = temp['Text'][comp.ObjectID];
                }
                else if (comp.ObjectType === 'Video') {
                    comp['Video'] = temp['Video'][comp.ObjectID] || new Model['Video']({ VideoID: -1 });
                }
                else if (comp.ObjectType === 'Dynamic') {
                    comp['Dynamic'] = temp['Dynamic'][comp.ObjectID];
                }
                temp['ComponentInfo'][comp.ComponentID] = comp;
            }
            PageData['PageInfo']['ComponentInfo'] = temp['ComponentInfo'];
            angular.extend(PageData['PageInfo'], data.PageInfo);
        }
        this.insert = function (ObjectType) {
            var CompID = this.newCompID();
            var comp = new Model['c']({ ObjectID: -1, PageID: PageData.PageInfo.PageID, ObjectType: ObjectType, ComponentID: CompID });
            switch (ObjectType) {
                case 'Text':
                    var text = new Model['Text']({ TextID: -1 });
                    comp['Text']= text;
                    break;
                case 'Video':
                    var video = new Model['Video']({ VideoID: -1});
                    comp['Video'] = video;
                    break;
                case 'Image':
                    var image = new Model['Image']({ ImageID: -1});
                    comp['Image'][0] = image;
                    break;
                case 'Dynamic':
                    var dynamic = new Model['Dynamic']({ DataID: -1 });
                    comp['Dynamic'] = dynamic;
                    break;
            }
            PageData.PageInfo.ComponentInfo[CompID] = comp;
        }
        this.newCompID = function () {
            var ftComps = $filter('orderBy')(KeypairToArray(PageData.PageInfo.ComponentInfo), 'ComponentID', 'reverse');
            if (ftComps.length > 0) {
                return ftComps[0]['ComponentID'] + 1;
            }
            else {
                return 1;
            }
        };
    })
    .service('SliderService', function (Slider, $timeout) {
        this.reloadSlider = function (slider, Object) {
            slider.reloadSlider({
                mode: 'fade',
                auto: true,
                autoHover: true,
                adaptiveHeight: true,
                pause: Object.Speed * 1000,
                speed: 1000,
                onSlideBefore: function ($slideElement, oldIndex, newIndex) {
                    slider.startAuto();
                }
            });
        }
        this.setSlider = function (slider, Object, ComponentID) {
            $timeout(function () {
                slider = $('#slider' + ComponentID).bxSlider({
                    mode: 'fade',
                    auto: true,
                    autoHover: true,
                    adaptiveHeight: true,
                    pause: Object.Speed * 1000,
                    speed: 1000,
                    onSlideBefore: function ($slideElement, oldIndex, newIndex) {
                        slider.startAuto();
                    }
                });
            
                Slider[ComponentID] = slider;
                $('#slider' + ComponentID).parent().next('.bx-controls').find('.bx-controls-direction').find('a').hide();
                $('#slider' + ComponentID).parent().parent().hover(function () {
                    $('#slider' + ComponentID).parent().next('.bx-controls').find('.bx-controls-direction').find('a').fadeIn('fast');
                }, function () {
                    $('#slider' + ComponentID).parent().next('.bx-controls').find('.bx-controls-direction').find('a').fadeOut('fast');
                });
            }, 0);
        }
    })
    ;
