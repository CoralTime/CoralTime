import { ODataConfiguration } from './../services/odata';

export function ODataConfigFactory() {
	let config = new ODataConfiguration();
	config.baseUrl = '/api/v1/odata';

	return config;
}
