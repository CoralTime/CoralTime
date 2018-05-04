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
		transition('left => void', [
			style({
				position: 'relative',
				left: 'calc(-100% - 5px)',
				opacity: 1
			}),
			animate('.5s ease',
				style({
					position: 'relative',
					left: '-130%',
					opacity: 0
				})
			)
		]),
		transition('void => left', [
			style({
				position: 'relative',
				left: '30%',
				opacity: 0
			}),
			animate('.5s ease',
				style({
					left: '0',
					opacity: 1
				})
			)
		]),
		transition('right => void', [
			style({
				position: 'relative',
				left: 'calc(-100% - 5px)',
				opacity: 1
			}),
			animate('.5s ease',
				style({
					position: 'relative',
					left: '-70%',
					opacity: 0
				})
			)
		]),
		transition('void => right', [
			style({
				position: 'relative',
				left: '-30%',
				opacity: 0
			}),
			animate('.5s ease',
				style({
					left: '0',
					opacity: 1
				})
			)
		])
	])
};
