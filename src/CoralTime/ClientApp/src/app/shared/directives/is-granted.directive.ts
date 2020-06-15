import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AclService } from '../../core/auth/acl.service';
import { AuthService } from '../../core/auth/auth.service';

@Directive({selector: '[ctIsGranted]'})
export class IsGrantedDirective {
	private isGranded: boolean = false;
	private role: string;

	constructor(private aclService: AclService,
	            private authService: AuthService,
	            private templateRef: TemplateRef<any>,
	            private viewContainer: ViewContainerRef) {
		this.authService.onChange.subscribe(authUser => {
			this.checkIsGranted();
		});

		this.authService.adminOrManagerParameterOnChange.subscribe(() => {
			this.checkIsGranted();
		});
	}

	@Input()
	set ctIsGranted(role: string) {
		this.role = role;
		this.checkIsGranted();
	}

	checkIsGranted(): void {
		let newIsGranted = !this.role || this.aclService.isGranted(this.role);

		if (newIsGranted && !this.isGranded) {
			this.viewContainer.createEmbeddedView(this.templateRef);
		} else if (!newIsGranted && this.isGranded) {
			this.viewContainer.clear();
		}

		this.isGranded = newIsGranted;
	}
}
