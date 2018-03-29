import { Http, HttpModule, BrowserXhr } from '@angular/http';
import { NgModule, ErrorHandler } from '@angular/core';
import { ODataServiceFactory, ODataConfiguration } from './../services/odata';
import { CustomHttp } from './custom-http';
import { ConstantService } from './constant.service';
import { AuthGuard } from './auth/auth-guard.service';
import { AuthService } from './auth/auth.service';
import { NotificationService } from './notification.service';
import * as ODataConfig from './odata-config.factory';
import { NotAuthGuard } from './auth/not-auth-guard.service';
import { LoadingIndicatorService } from './loading-indicator.service';
import { CustomBrowserXhr } from './custom-browser-xhr';
import { AclService } from './auth/acl.service';
import { CustomErrorHandler } from './raven-error-handler';
import { UserPicService } from '../services/user-pic.service';

@NgModule({
	imports: [
		HttpModule,
	],
	exports: [
		HttpModule
	],
	providers: [
		{
			provide: BrowserXhr,
			useClass: CustomBrowserXhr
		},
		{
			provide: Http,
			useClass: CustomHttp
		},
		{
			provide: ErrorHandler,
			useClass: CustomErrorHandler
		},
		{
			provide: ODataConfiguration,
			useFactory: ODataConfig.ODataConfigFactory
		},
		AclService,
		AuthService,
		AuthGuard,
		ConstantService,
		CustomHttp,
		LoadingIndicatorService,
		NotAuthGuard,
		NotificationService,
		ODataServiceFactory,
		UserPicService
	]
})

export class CoreModule {
}
