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

export const ctCalendarAnimations: {
	readonly slideCalendar: AnimationTriggerMetadata;
} = {
	slideCalendar: trigger('slideCalendar', [
		// Note: The `enter` animation doesn't transition to something like `translate3d(0, 0, 0)
		// scale(1)`, because for some reason specifying the transform explicitly, causes IE both
		// to blur the dialog content and decimate the animation performance. Leaving it as `none`
		// solves both issues.
		// state('*', style({transform: 'none', opacity: 1})),
		// state('void', style({transform: 'translate3d(100%, 0, 0)', opacity: 1})),
		// state('exit', style({transform: 'translate3d(100%, 0, 0)', opacity: 1})),
		//
		// transition('* <=> *',
		// 	// query(':leave', [
		// 	// 	animate(3000, style({transform: 'translate3d(100%, 0, 0)'}))
		// 	// ]),
		// 	animate('400ms cubic-bezier(0.25, 0.8, 0.25, 1)',
		// 		// style({transform: 'translate3d(0%, 0, 0)', opacity: 1})
		// 	)
		// ),
	])
};
