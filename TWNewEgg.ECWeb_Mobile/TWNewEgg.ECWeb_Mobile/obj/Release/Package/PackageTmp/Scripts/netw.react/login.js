!function(e){function t(n){if(a[n])return a[n].exports;var r=a[n]={exports:{},id:n,loaded:!1};return e[n].call(r.exports,r,r.exports,t),r.loaded=!0,r.exports}var a={};return t.m=e,t.c=a,t.p="",t(0)}({0:function(e,t,a){e.exports=a(83)},83:function(e,t,a){"use strict";var n=a(84),r=function(){ReactDOM.render(React.createElement(n,null),document.getElementById("content"))};window.nemReact.reacts.login={init:r}},84:function(e,t){"use strict";var a=React.createClass({displayName:"LoginContent",getLoginData:function(e){var t=/^([\w-\.\+\-\_]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4})$/;if(null!=this.state.userAccount){if(!this.state.userAccount.match(t))return void this.setState({userAccountCheck:!1,userAccountErrMsg:"請輸入正確E-Mail帳號"});this.setState({userAccountCheck:!0}),document.getElementById("loginForm").submit()}else this.setState({userAccountCheck:!1,userAccountErrMsg:"請輸入E-Mail帳號"})},getInitialState:function(){return{userAccount:netwRespMessage.user,userAccountCheck:!0,userAccountErrMsg:"",userPassword:"",response:netwRespMessage}},handleInputText:function(e){var t=e.target.name,a=e.target.value;switch(t){case"user":this.setState({userAccount:a});break;case"pass":this.setState({userPassword:a})}},checkUserError:function(){var e="";if(0==!!this.state.userAccountCheck)return e="errMsg"},checkPwdError:function(){var e="";if(this.state.response.Pwderro)return e="errMsg"},render:function(){return React.createElement("div",{className:"login"},React.createElement("div",{className:"title"},React.createElement("h3",null,"新蛋全球生活網會員登入")),React.createElement("div",{className:"loginPage"},React.createElement("form",{id:"loginForm",action:nemReact.generateUrl("login"),method:"post"},React.createElement("ul",{className:"loginContent"},React.createElement("li",null,React.createElement("p",{className:this.checkUserError()},React.createElement("input",{type:"Email",name:"user",defaultValue:this.state.userAccount,placeholder:"請輸入e-mail(新蛋帳號)",onChange:this.handleInputText}),React.createElement("span",null,this.state.userAccountErrMsg)),React.createElement("p",{className:this.checkPwdError()},React.createElement("input",{type:"password",name:"pass",defaultValue:this.state.userPassword,placeholder:"請輸入密碼(8-12碼數字或英文字母混合)",onChange:this.handleInputText}),React.createElement("span",null,this.state.response.Pwderro)),React.createElement("a",{className:"loginBtn",onClick:this.getLoginData},"登入"),React.createElement("p",{className:"forgetPwd"},React.createElement("a",{href:_netwWebURL+nemReact.generateUrl("forgetpass")},"忘記密碼?")),React.createElement("a",{className:"registerBtn",href:nemReact.generateUrl("signup")},"申請註冊"),React.createElement("p",{className:"desktopView",style:{display:"none"}},React.createElement("a",{href:_netwWebURL},"檢視桌上型電腦版")))),React.createElement("input",{type:"hidden",id:"ratm",name:"ratm",value:this.state.response.ratm}),React.createElement("input",{type:"hidden",id:"type",name:"type",value:this.state.response.type}),React.createElement("input",{type:"hidden",id:"acty",name:"acty",value:this.state.response.acty}),React.createElement("input",{type:"hidden",id:"returnUrl",name:"returnUrl",value:netwRespReturnUrl}))))}});e.exports=a}});