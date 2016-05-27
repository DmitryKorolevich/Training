"use strict";

var gulp = require("gulp"),
    seq = require("run-sequence"),
    bower = require("gulp-main-bower-files"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    fs = require('fs'),
    less = require('gulp-less'),
    merge = require('merge-stream'),
    print = require('gulp-print'),
    uglify = require("gulp-uglify");

var jsConfig = JSON.parse(fs.readFileSync('./AppConfig/scripts/files.json'));
var cssConfig = JSON.parse(fs.readFileSync('./AppConfig/styles/files.json'));

var jsFiles = jsConfig.files;
for (var i = 0; i < jsFiles.length; i++) {
    jsFiles[i] = "wwwroot/" + jsFiles[i];
}

var cssFiles = cssConfig.files;
for (var j = 0; j < cssFiles.length; j++) {
    cssFiles[j] = "wwwroot/" + cssFiles[j];
}

cssFiles.push('./temp/less/*.css');

gulp.task("default", ["bower"]);

gulp.task("development", function (callBack) {
    seq(['clean:assets', 'clean:app', 'clean:js', 'clean:css', 'bower'], 'less', 'copy:dev', 'clean:temp', callBack);
});

gulp.task("release", function (callBack) {
    seq(['clean:assets', 'clean:app', 'clean:js', 'clean:css', 'bower'], 'less', 'copy:dev', 'min', ['clean:assets', 'clean:app'], 'copy:rel', 'clean:temp', callBack);
});

gulp.task("bower", function () {
    return gulp.src('./bower.json')
        .pipe(bower())
        .pipe(gulp.dest('./wwwroot/lib/'));
});

gulp.task("copy:dev", function () {
    var result = merge();
    result.add(gulp.src(["./app/**/*.*", ]).pipe(gulp.dest("./wwwroot/app/")));
    result.add(gulp.src(["./assets/scripts/**/*.*", ]).pipe(gulp.dest("./wwwroot/assets/scripts/")));
    result.add(gulp.src(["./temp/less/**/*.*", ]).pipe(gulp.dest("./wwwroot/assets/styles/")));
    result.add(gulp.src(["./assets/images/**/*.*", ]).pipe(gulp.dest("./wwwroot/assets/images/")));
    result.add(gulp.src(["./assets/fonts/**/*.*", ]).pipe(gulp.dest("./wwwroot/assets/fonts/")));
    result.add(gulp.src(["./assets/miscellaneous/**/*.*", ]).pipe(gulp.dest("./wwwroot/assets/miscellaneous/")));
    return result;
});

gulp.task("copy:rel", function () {
    var result = merge();
    result.add(gulp.src(["./app/**/*.*", ]).pipe(gulp.dest("./wwwroot/app/")));
    result.add(gulp.src(["./assets/scripts/vendor/jquery-ui/images/**/*.*", ]).pipe(gulp.dest("./wwwroot/images/")));
    result.add(gulp.src(["./assets/images/**/*.*", ]).pipe(gulp.dest("./wwwroot/assets/images/")));
    result.add(gulp.src(["./assets/fonts/**/*.*", ]).pipe(gulp.dest("./wwwroot/assets/fonts/")));
    result.add(gulp.src(["./assets/miscellaneous/**/*.*", ]).pipe(gulp.dest("./wwwroot/assets/miscellaneous/")));
    return result;
});

gulp.task("less", function () {
    return gulp.src([
        './Assets/styles/main.less',
        './Assets/styles/site.less',
        './Assets/styles/font-awesome/font-awesome.less'
    ]).pipe(less())
    .pipe(gulp.dest("./temp/less/"));
});

gulp.task("min:js", function () {
    return gulp.src(jsFiles)
        .pipe(print())
        .pipe(concat(jsConfig.minifiedFileName + ".min.js"))
        .pipe(uglify())
        .pipe(gulp.dest("./wwwroot/"));
});

gulp.task("min:css", function () {
    return gulp.src(cssFiles)
        .pipe(print())
        .pipe(concat(cssConfig.minifiedFileName + ".min.css"))
        .pipe(cssmin())
        .pipe(gulp.dest("./wwwroot/"));
});

gulp.task("clean:js", function (cb) {
    rimraf("./wwwroot/*.js", cb);
});

gulp.task("clean:css", function (cb) {
    rimraf("./wwwroot/*.css", cb);
});

gulp.task("clean:app", function (cb) {
    rimraf("./wwwroot/app/**", cb);
});

gulp.task("clean:assets", function (cb) {
    rimraf("./wwwroot/assets/**", cb);
});

gulp.task("clean:app", function (cb) {
    rimraf("./wwwroot/app/**", cb);
});

gulp.task("clean:temp", function (cb) {
    rimraf("./temp/**", cb);
});

gulp.task("min", ["min:js", "min:css"]);