define(function(){return function(t){var e={};function n(r){if(e[r])return e[r].exports;var i=e[r]={i:r,l:!1,exports:{}};return t[r].call(i.exports,i,i.exports,n),i.l=!0,i.exports}return n.m=t,n.c=e,n.d=function(t,e,r){n.o(t,e)||Object.defineProperty(t,e,{enumerable:!0,get:r})},n.r=function(t){"undefined"!=typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(t,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(t,"__esModule",{value:!0})},n.t=function(t,e){if(1&e&&(t=n(t)),8&e)return t;if(4&e&&"object"==typeof t&&t&&t.__esModule)return t;var r=Object.create(null);if(n.r(r),Object.defineProperty(r,"default",{enumerable:!0,value:t}),2&e&&"string"!=typeof t)for(var i in t)n.d(r,i,function(e){return t[e]}.bind(null,i));return r},n.n=function(t){var e=t&&t.__esModule?function(){return t.default}:function(){return t};return n.d(e,"a",e),e},n.o=function(t,e){return Object.prototype.hasOwnProperty.call(t,e)},n.p="",n(n.s=1)}([,function(t,e,n){var r;void 0===(r=function(t,e){"use strict";Object.defineProperty(e,"__esModule",{value:!0});var n=function(){function t(t,e){this.WidgetHelpers=t,this.isSprintWidget=e,this.widgetConfigurationContext=null,this.extentionSettings=null,this.$siteUrl=$("#siteUrl-input"),this.$userName=$("#userName-input"),this.$password=$("#password-input"),this.$isSSO=$("#isSSO-input"),this.currentIterationEnd=null}return t.prototype.load=function(t,e){this.widgetConfigurationContext=e;JSON.parse(t.customSettings.data);return this.extentionSettings=this.getExtensionContext(),this.WidgetHelpers.WidgetStatusHelper.Success()},t.prototype.onSave=function(){return this.WidgetHelpers.WidgetConfigurationSave.Valid(this.getCustomSettings())},t.prototype.getCustomSettings=function(){var t=this.$siteUrl.val(),e=this.$userName.val(),n=this.$password.val(),r=this.$isSSO.prop("checked");return{data:JSON.stringify({isSSO:r,password:n,siteUrl:t,userName:e})}},t.prototype.getExtensionContext=function(){var t=VSS.getWebContext();return{projectId:t.project.id,projectName:t.project.name,userEmail:t.user.email,userName:t.user.uniqueName}},t.$dateTimeCombo=null,t}();e.Configuration=n}.apply(e,[n,e]))||(t.exports=r)}])});