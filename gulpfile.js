var gulp = require('gulp'),
	rename = require('gulp-rename'),
	del = require('del'),
	watch = require('gulp-watch');
gulp.task('clean:vaults', function() {
	return del([
		'Assets/Resources/Vaults/**/*'
	]);
});
gulp.task('convert:vaults', ['clean:vaults'], function() {
	gulp.src('Assets/Vaults/**/*.tmx').pipe(
		rename(function(path) {
			path.extname = '.xml'
		})
	).pipe(
		gulp.dest('Assets/Resources/Vaults/')
	);
});

gulp.task('stream', ['convert:vaults'], function() {
	return watch('Assets/Vaults/**/*.tmx')
	.pipe(
		rename(function(path) {
			path.extname = '.xml'
		})
	).pipe(
		gulp.dest('Assets/Resources/Vaults/')
	);
});
gulp.task('default', ['stream']);
