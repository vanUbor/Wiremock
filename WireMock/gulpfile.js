const gulp = require('gulp');
const ts = require('gulp-typescript');

const tsProject = ts.createProject('tsconfig.json');

function typescript() {
    return tsProject.src()
        .pipe(tsProject())
        .js.pipe(gulp.dest('wwwroot/js'));
}

gulp.watch('wwwroot/scripts/**/*.ts', typescript);


exports.default = gulp.series(typescript);