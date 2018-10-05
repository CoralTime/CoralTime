var gulp = require('gulp');
var inlinesource = require('gulp-inline-source');

gulp.task('inlinesource', function () {
    return gulp.src('./coverage/**/*.html')
        .pipe(inlinesource({attribute: false}))
        .pipe(gulp.dest('./coverage-inline'));
});