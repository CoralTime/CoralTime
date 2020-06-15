import { AppInsightsService } from '@markpieszak/ng-application-insights';
import { ODataConfiguration } from '../services/odata';

export function ODataConfigFactory(appInsightsService: AppInsightsService) {
	let config = new ODataConfiguration(appInsightsService);
	config.baseUrl = '/api/v1/odata';

	return config;
}
