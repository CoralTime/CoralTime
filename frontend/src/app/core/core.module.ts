import { HttpModule, BrowserXhr } from '@angular/http';
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
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { ApplyTokenInterceptor } from './apply-token.interceptor';
import { RefreshTokenInterceptor } from './refresh-token.interceptor';

@NgModule({
	imports: [
		HttpModule,
		HttpClientModule
	],
	exports: [
		HttpModule
	],
	providers: [
		{
			provide: ErrorHandler,
			useClass: CustomErrorHandler
		},
		{
			provide: ODataConfiguration,
			useFactory: ODataConfig.ODataConfigFactory
		},
		{
			provide: HTTP_INTERCEPTORS,
			useClass: ApplyTokenInterceptor,
			multi: true
		},
		{
			provide: HTTP_INTERCEPTORS,
			useClass: RefreshTokenInterceptor,
			multi: true
		},
		AclService,
		AuthService,
		AuthGuard,
		ConstantService,
		CustomHttp,
		HttpClientModule,
		LoadingIndicatorService,
		NotAuthGuard,
		NotificationService,
		ODataServiceFactory,
		UserPicService
	]
})

export class CoreModule {
}
