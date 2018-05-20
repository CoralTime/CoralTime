import { NgModule, ErrorHandler } from '@angular/core';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { LoadingBarService } from '@ngx-loading-bar/core';
import * as ODataConfig from './odata-config.factory';
import { ODataServiceFactory, ODataConfiguration } from './../services/odata';
import { ConstantService } from './constant.service';
import { AuthGuard } from './auth/auth-guard.service';
import { AuthService } from './auth/auth.service';
import { NotificationService } from './notification.service';
import { NotAuthGuard } from './auth/not-auth-guard.service';
import { AclService } from './auth/acl.service';
import { CustomErrorHandler } from './raven-error-handler';
import { UserPicService } from '../services/user-pic.service';
import { ApplyTokenInterceptor } from './apply-token.interceptor';
import { RefreshTokenInterceptor } from './refresh-token.interceptor';
import { LoadingMaskModule } from '../shared/loading-indicator/loading-mask.module';

@NgModule({
	imports: [
		HttpClientModule,
		LoadingMaskModule
	],
	exports: [
		HttpClientModule,
		LoadingMaskModule
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
		LoadingBarService,
		NotAuthGuard,
		NotificationService,
		ODataServiceFactory,
		UserPicService
	]
})

export class CoreModule {
}
