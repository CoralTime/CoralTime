import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { AclService } from '../../core/auth/acl.service';
import { AuthService } from '../../core/auth/auth.service';
import { ImpersonationService } from '../../services/impersonation.service';

@Directive({selector: '[ctIsGranted]'})
export class IsGrantedDirective {
	private isGranded: boolean = false;
	private policy: string;

	constructor(private aclService: AclService,
	            private authService: AuthService,
	            private impersonationService: ImpersonationService,
	            private templateRef: TemplateRef<any>,
	            private viewContainer: ViewContainerRef) {
		this.authService.onChange.subscribe(authUser => {
			this.checkIsGranted();
		});

		this.impersonationService.onChange.subscribe(() => {
			this.checkIsGranted();
		});
	}

	@Input()
	set ctIsGranted(policy: string) {
		this.policy = policy;
		this.checkIsGranted();
	}

	checkIsGranted(): void {
		let newIsGranted = !this.policy || this.aclService.isGranted(this.policy);

		if (newIsGranted && !this.isGranded) {
			this.viewContainer.createEmbeddedView(this.templateRef);
		} else if (!newIsGranted && this.isGranded) {
			this.viewContainer.clear();
		}

		this.isGranded = newIsGranted;
	}
}
