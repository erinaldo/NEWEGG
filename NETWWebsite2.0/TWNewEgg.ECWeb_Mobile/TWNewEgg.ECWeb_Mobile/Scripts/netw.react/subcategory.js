!function(e){function t(a){if(r[a])return r[a].exports;var s=r[a]={exports:{},id:a,loaded:!1};return e[a].call(s.exports,s,s.exports,t),s.loaded=!0,s.exports}var r={};return t.m=e,t.c=r,t.p="",t(0)}({0:function(e,t,r){e.exports=r(126)},50:function(e,t){"use strict";var r=React.createClass({displayName:"Loading",loadStyle:function(){var e={height:"60px"};return"number"==typeof this.props.height&&(e.height=this.props.height+"px"),e},showContent:function(){var e=React.createElement("div",{className:"load"},React.createElement("span",null,"Loading..."));return"string"==typeof this.props.content&&(e=this.props.content),e},render:function(){return React.createElement("div",{style:this.loadStyle()},this.showContent())}});e.exports=r},116:function(e,t){"use strict";var r=[{orderMenu:"newArrival",orderName:"最新上架"},{orderMenu:"popularity",orderName:"人氣排行榜"},{orderMenu:"recommended",orderName:"推薦排行"},{orderMenu:"highPrice",orderName:"金額高"},{orderMenu:"lowPrice",orderName:"金額低"}],a=React.createClass({displayName:"ProductInLi",render:function(){return React.createElement("li",null,this.props.product)}}),s=React.createClass({displayName:"SelectFrameProds",setProdStyle:function(e){this.setState({fullProdStyle:e})},showProducts:function(){var e=[];if("undefined"==typeof this.state.productLists||0==this.state.productLists.length&&!this.state.isLoad){var t=React.createElement("div",{key:"noProducts"},this.state.noProductWords);return e.push(t),e}for(var r=0;r<this.state.productLists.length;r++){var t=React.createElement(a,{key:r,product:this.state.productLists[r]});e.push(t)}return e},propTypes:{productLists:React.PropTypes.array,noProductWords:React.PropTypes.string,orderList:React.PropTypes.array.isRequired},handleSelectOrder:function(e){this.setState({currentOrderMenu:e.target.value}),setTimeout(function(){this.props.setOrder(this.state.currentOrderMenu)}.bind(this),200)},getInitialState:function(){return{orderSelect:r,currentOrderMenu:r[0].orderMenu,fullProdStyle:!1,productLists:this.props.productLists,noProductWords:this.props.noProductWords,isLoad:this.props.isLoad}},componentWillMount:function(){this.setState({orderSelect:this.props.orderList,currentOrderMenu:this.props.orderMenu})},componentWillReceiveProps:function(e){this.setState({productLists:e.productLists,noProductWords:e.noProductWords,currentOrderMenu:e.orderMenu,isLoad:e.isLoad})},componentDidMount:function(){},render:function(){return React.createElement("div",{className:"SelectFramePage"},React.createElement("div",{className:"selectFrame"},React.createElement("p",{className:"selectWrap noLabel"},React.createElement("select",{value:this.state.currentOrderMenu,onChange:this.handleSelectOrder},this.state.orderSelect.map(function(e,t){return React.createElement("option",{key:t,value:e.orderMenu},e.orderName)},this))),React.createElement("p",{className:"frameTwo fa "+(this.state.fullProdStyle?"frameColor":""),onClick:this.setProdStyle.bind(this,!0)}),React.createElement("p",{className:"frameOne fa "+(this.state.fullProdStyle?"":"frameColor"),onClick:this.setProdStyle.bind(this,!1)})),React.createElement("div",{className:"subprodLists frameList"},React.createElement("ul",{className:"prodLists "+(this.state.fullProdStyle?"full":""),key:this.props.prodListKey},this.showProducts())))}});e.exports=s},126:function(e,t,r){"use strict";var a=r(127),s=function(){ReactDOM.render(React.createElement(a,null),document.getElementById("content"))};window.nemReact.reacts.subcategory={init:s}},127:function(e,t,r){"use strict";function a(e){var t=m.getAll();return null!=t&&0!==Object.keys(t).length||h.getItemParentCate(e),t}function s(e,t,r){var a=l.getSubItems();return null!=a&&0!==a.length||d.getSubItems(e,t,r),{subItems:a}}var n=nemReact.require("NEProduct","react"),o=r(128),i=r(116),c=nemReact.require("NetwTools"),u=r(50),d=nemReact.require("SubCategoryAction"),l=nemReact.require("SubCategoryStore"),h=nemReact.require("CategoryParentAction"),m=nemReact.require("CategoryParentStore"),p=[{orderMenu:"CreatDate",orderName:"最新上架"},{orderMenu:"PopularityIndex",orderName:"人氣排行榜"},{orderMenu:"Recommended",orderName:"推薦排行"},{orderMenu:"HighPrice",orderName:"金額高"},{orderMenu:"LowPrice",orderName:"金額低"}],g=React.createClass({displayName:"SubCategoryParent",handleSelectChange:function(e){var t=e.target.value;this.props.handleSelectChange(t)},fetchDropDownItems:function(e){var t=[];if(!e.hasOwnProperty("DropDownItems"))return t;t.length=0;for(var r=0;r<e.DropDownItems.length;r++){var a={subCateId:e.DropDownItems[r].CategoryID,subCateName:e.DropDownItems[r].Title};t.push(a)}return t},getInitialState:function(){return{subCateData:[],currentSubCateId:0}},componentWillMount:function(){m.addChangeListener(this._onChange),this.setState({subCateData:this.fetchDropDownItems(a(this.props.currentSubCategoryID))})},componentDidMount:function(){},componentWillUnmount:function(){m.removeChangeListener(this._onChange)},render:function(){return React.createElement("div",{className:"selectWrap noLabel"},React.createElement("select",{value:this.props.currentSubCategoryID,onChange:this.handleSelectChange},this.state.subCateData.map(function(e,t){return React.createElement("option",{key:t,value:e.subCateId},e.subCateName)},this)))},_onChange:function(){var e=a(this.props.currentSubCategoryID);null!=e&&this.setState({subCateData:this.fetchDropDownItems(e)})}}),I=React.createClass({displayName:"SubcategoryContent",setOrder:function(e){var t="OrderBy="+e+"&CategoryID="+this.state.currentSubCateId;history.pushState({categoryID:this.state.currentSubCateId,order:e},"","?"+t),this.setState({order:e,pageNumber:1,subItems:[],isLoad:!0}),setTimeout(function(){d.getSubItems(this.state.currentSubCateId,this.state.order,this.state.pageNumber)}.bind(this),200)},handleScroll:function(e){var t=$("#subCategory").height(),r=window.innerHeight,a=$(window).scrollTop(),s=a+r;if(s+100>t)if(!this.state.isLoad&&l.getHasData()){this.setState({isLoad:!0});var n=l.getMaxPage(),o=++this.state.pageNumber;n>=o?d.getSubItems(this.state.currentSubCateId,this.state.order,this.state.pageNumber):this.setState({isLoad:!1})}else this.setState({isLoad:!1})},handleOnPopState:function(e){if(e.state)this.setState({currentSubCateId:e.state.categoryID,order:e.state.order,pageNumber:1,isLoad:!0}),setTimeout(function(){d.getSubItems(this.state.currentSubCateId,this.state.order,1)}.bind(this),200);else{var t=c.parseUrlFormatByNameIC(location.search,"orderby");t||(t="CreateDate");var r=c.parseUrlFormatByNameIC(location.search,"categoryid");this.setState({currentSubCateId:r,order:t,pageNumber:1,isLoad:!0}),setTimeout(function(){d.getSubItems(this.state.currentSubCateId,this.state.order,1)}.bind(this),200)}},handleSelectChange:function(e){var t="OrderBy="+this.state.order+"&CategoryID="+e;history.pushState({categoryID:e,order:this.state.order},"","?"+t),this.setState({currentSubCateId:e,pageNumber:1,subItems:[],isLoad:!0}),setTimeout(function(){d.getSubItems(this.state.currentSubCateId,this.state.order,this.state.pageNumber)}.bind(this),200)},showLoad:function(){return this.state.isLoad?React.createElement(u,null):null},generateNEProd:function(){for(var e=[],t=0;t<this.state.subItems.length;t++){var r=React.createElement(n,{productID:this.state.subItems[t].ID,imgUrl:this.state.subItems[t].imgPath,productLink:"",title:this.state.subItems[t].Name,qty:this.state.subItems[t].SellingQty,marketPrice:this.state.subItems[t].MarketPrice,sellingPrice:this.state.subItems[t].PriceCash});e.push(r)}return e},getInitialState:function(){var e=c.parseUrlFormatByNameIC(location.search,"orderby");return e||(e="CreateDate"),{currentSubCateId:c.parseUrlFormatByNameIC(location.search,"categoryid"),isLoad:!0,order:e,pageNumber:1,subItems:[]}},componentWillMount:function(){window.addEventListener("scroll",this.handleScroll),window.addEventListener("popstate",this.handleOnPopState),l.addChangeListener(this._onChange),this.setState(s(this.state.currentSubCateId,this.state.order,this.state.pageNumber))},componentDidMount:function(){},componentWillUnmount:function(){l.removeChangeListener(this._onChange),window.removeEventListener("scroll",this.handleScroll),window.removeEventListener("popstate",this.handleOnPopState)},render:function(){return React.createElement("div",{className:"category subCates",id:"subCategory"},React.createElement("div",{className:"title"},React.createElement(g,{currentSubCategoryID:this.state.currentSubCateId,handleSelectChange:this.handleSelectChange})),React.createElement("div",{className:"CategoryContent"},React.createElement(o,{currentCID:this.state.currentSubCateId}),React.createElement(i,{noProductWords:"Sorry，此處建構中...",productLists:this.generateNEProd(),prodListKey:this.state.currentSubCateId,orderList:p,orderMenu:this.state.order,setOrder:this.setOrder,isLoad:this.state.isLoad}),this.showLoad()))},_onChange:function(){this.setState({isLoad:!1}),l.getHasData()&&this.setState(s(this.state.currentSubCateId,this.state.order,this.state.pageNumber))}});e.exports=I},128:function(e,t){"use strict";function r(e){var t=n.getTopTen(e);return 0===Object.keys(t).length&&s.getTopTen(e),t}var a=nemReact.require("NEProduct","react"),s=nemReact.require("SubCategoryTopTenAction"),n=nemReact.require("SubCategoryTopTenStore"),o=nemReact.require("NetwTools"),i=React.createClass({displayName:"TopTenItemDom",generateDom:function(e,t){if("undefined"==typeof t)return null;var r=++e;return React.createElement("li",{className:"prods prodsBig top"+(r>9?""+r:"0"+r)},React.createElement(a,{productID:t.ItemID,imgUrl:o.isHttpUriAbsolute(t.ItemImage)?t.ItemImage:_netwImageSSLDM+t.ItemImage,productLink:"#",title:t.Title,qty:t.SellingQty,marketPrice:t.MarketPrice,sellingPrice:t.UnitPrice}))},render:function(){return React.createElement("ul",{className:"prodTop10List prodLists ranks"},this.generateDom(this.props.first,this.props.firstData),this.generateDom(this.props.second,this.props.secondData))}}),c=React.createClass({displayName:"TopTenRoot",generateTopItem:function(){var e=[];if(!this.state.topTenItems)return e;for(var t=0;t<this.state.topTenItems.length;t+=2){var r=React.createElement(i,{key:t,first:t,second:t+1,firstData:this.state.topTenItems[t],secondData:this.state.topTenItems[t+1]});e.push(r)}return e},getInitialState:function(){return{topTenItems:this.props.topTenItems,categoryID:this.props.categoryID,isSlick:!1}},componentWillUnmount:function(){},componentDidMount:function(){},componentWillReceiveProps:function(e){var t=this.state.isSlick;t?($(".subCatesTop10_Slide").slick("unslick"),$(".subCatesTop10_Slide").slick({dots:!0}),t=!0):($(".subCatesTop10_Slide").slick({dots:!0}),t=!0),this.state.categoryID!=e.categoryID&&($(".subCatesTop10_Slide").slick("unslick"),t=!1),this.setState({topTenItems:e.topTenItems,categoryID:e.categoryID,isSlick:t})},render:function(){return"undefined"==typeof this.state.topTenItems||0==this.state.topTenItems.length?React.createElement("div",{className:"module slider single-item subCatesTop10_Slide"}):React.createElement("div",{className:"module slider single-item subCatesTop10_Slide"},this.generateTopItem())}}),u=React.createClass({displayName:"Top10",generateTopTenSlick:function(){var e=React.createElement(c,{topTenItems:this.state.topTenItems,categoryID:this.state.categoryID});return e},showTopTen:function(){var e={};return this.state.topTenItems&&0!=this.state.topTenItems.length||(e.display="none"),e},getInitialState:function(){return{categoryID:this.props.currentCID,isLoad:!0,topTenItems:[],isSlick:!0}},componentWillMount:function(){n.addChangeListener(this._onChange);var e=r(this.state.categoryID);this.setState({topTenItems:e.topTenData})},componentDidMount:function(){},componentWillReceiveProps:function(e){var t=r(e.currentCID);this.setState({categoryID:e.currentCID,topTenItems:t.topTenData})},componentWillUnmount:function(){n.removeChangeListener(this._onChange)},render:function(){return React.createElement("div",{className:"Top10Page",style:this.showTopTen()},React.createElement("p",{className:"Top10Text"},"銷售TOP10"),React.createElement("div",{className:"prodTop10"},React.createElement(c,{topTenItems:this.state.topTenItems,categoryID:this.state.categoryID})))},componentDidUpdate:function(){},_onChange:function(){var e=r(this.state.categoryID);this.setState({isLoad:!1,topTenItems:e.topTenData})}});e.exports=u}});