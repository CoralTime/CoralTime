import { Roles } from './permissions';
import * as jwt_decode from 'jwt-decode';

export class AuthUser {
	readonly accessToken: string;
	readonly expiresIn: number;
	readonly id: number;
	readonly isManager: string;
	readonly isSso: boolean;
	readonly nickname: string;
	readonly refreshToken: string;
	readonly refreshTokenExpiration: number;
	readonly role: number;
	readonly tokenType: string;

	constructor(data, isSso: boolean) {
		this.accessToken = data.access_token;
		this.expiresIn = data.expires_in;
		this.isSso = isSso;
		this.refreshToken = data.refresh_token;
		this.tokenType = data.token_type;

		let decodedToken = jwt_decode(data.access_token);
		this.id = +decodedToken.id;
		this.isManager = decodedToken.isManager;
		this.nickname = decodedToken.nickname;
		this.refreshTokenExpiration = new Date().getTime() + decodedToken.refreshTokenLifeTime * 1000;
		let roleName = Array.isArray(decodedToken.role) ? decodedToken.role[0] : decodedToken.role;
		this.role = Roles[roleName];
	}
}
