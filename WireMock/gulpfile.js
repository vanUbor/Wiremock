const gulp = require('gulp');
const ts = require('gulp-typescript');
const sourcemaps = require('gulp-sourcemaps');

const tsProject = ts.createProject('tsconfig.json');

function typescript() {
    return tsProject.src()
        .pipe(sourcemaps.init())
        .pipe(tsProject())
        .pipe(sourcemaps.write('.', { includeContent: false, sourceRoot: '../scripts' }))
        .pipe(gulp.dest('wwwroot/js'));
}

gulp.watch('wwwroot/scripts/**/*.ts', typescript);


exports.default = gulp.series(typescript);