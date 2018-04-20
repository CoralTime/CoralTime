import {
	animate,
	state,
	group,
	style,
	transition,
	trigger,
	query,
	animateChild,
	AnimationTriggerMetadata,
} from '@angular/animations';

export const ctRoutingAnimations: {
	readonly routerAnimation: AnimationTriggerMetadata;
} = {
	routerAnimation: trigger('routerAnimation', [
		transition('Reports => Calendar', [
			// Initial state of new route
			query(':enter',
				style({
					position: 'fixed',
					// width: '100%',
					width: 'calc(100% - 40px)',
					opacity: 0,
					transform: 'translateX(-100%)'
				})
			),

			// move page off screen right on leave
			query(':leave',
				animate('500ms ease',
					style({
						position: 'fixed',
						width: 'calc(100% - 40px)',
						opacity: 0,
						transform: 'translateX(100%)'
					})
				)
			),

			// move page in screen from left to right
			query(':enter',
				animate('400ms cubic-bezier(0.25, 0.8, 0.25, 1)',
					style({
						opacity: 1,
						transform: 'translateX(0)'
					})
				)
			),
		]),
		transition('Calendar => Reports', [
			// Initial state of new route
			query(':enter',
				style({
					position: 'fixed',
					width: 'calc(100% - 40px)',
					opacity: 0,
					transform: 'translateX(100%)'
				})
			),

			// move page off screen right on leave
			query(':leave',
				animate('500ms ease',
					style({
						position: 'fixed',
						width: 'calc(100% - 40px)',
						opacity: 0,
						transform: 'translateX(-100%)'
					})
				)),

			// move page in screen from left to right
			query(':enter',
				animate('400ms cubic-bezier(0.25, 0.8, 0.25, 1)',
					style({
						opacity: 1,
						transform: 'translateX(0)'
					})
				)
			),
		])
	])
};
