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
    html2js = require('gulp-ng-html2js'),
    htmlmin = require('gulp-minify-html'),
    uglify = require("gulp-uglify");

var jsConfig = JSON.parse(fs.readFileSync('./AppConfig/scripts/files.json'));
var cssConfig = JSON.parse(fs.readFileSync('./AppConfig/styles/files.json'));
var cssOrderInvoiceConfig = JSON.parse(fs.readFileSync('./AppConfig/styles-order-invoice/files.json'));

var jsFiles = jsConfig.files;
for (var i = 0; i < jsFiles.length; i++) {
    jsFiles[i] = "wwwroot/" + jsFiles[i];
}

var cssFiles = cssConfig.files;
for (var j = 0; j < cssFiles.length; j++) {
    cssFiles[j] = "wwwroot/" + cssFiles[j];
}
cssFiles.push('./temp/less/**/*.css');

var cssOrderInvoiceFiles = cssOrderInvoiceConfig.files;
for (var j = 0; j < cssOrderInvoiceFiles.length; j++)
{
	cssOrderInvoiceFiles[j] = "wwwroot/" + cssOrderInvoiceFiles[j];
}

gulp.task("default", ["bower"]);

gulp.task("development", function (callBack) {
    seq(['clean:assets', 'clean:app', 'clean:js', 'clean:css', 'bower'], ['less', 'html2js'], 'copy:dev', 'clean:temp', callBack);
});

gulp.task("release", function (callBack) {
    seq(['clean:assets', 'clean:app', 'clean:js', 'clean:css', 'bower'], ['less', 'html2js'], 'copy:dev', 'min', ['clean:assets', 'clean:app'], 'copy:rel', 'clean:temp', callBack);
});

gulp.task("bower", function () {
    return gulp.src('./bower.json')
        .pipe(bower({
            overrides: {
                "angular-promise-tracker": {
                    "main": "*.js"
                },
                "jsondiffpatch": {
                    "main": "public/**/*.*"
                },
                "ace-builds": {
                    "main": "./src-min-noconflict/**/*.*"
                },
                "ngprogress": {
                    "main": ["./build/ngprogress.js", "./ngProgress.css"]
                },
                "font-awesome": {
                    "main": ["fonts/*.*", "css/font-awesome.css"]
                },
                "bootstrap": {
                    "main": ["./dist/fonts/*.*", "./dist/js/bootstrap.js"]
                },
                "ladda": {
                    "main": ["./dist/**/*.*"]
                }
            }
        }))
        .pipe(gulp.dest('./wwwroot/lib/'));
});

//gulp.task("concat:styles", function () {
//    var result = merge();
//    result.add(gulp.src(cssFiles).pipe(concat(cssConfig.minifiedFileName + ".css")).pipe(gulp.dest("./wwwroot/assets/styles/")));
//    result.add(gulp.src(cssOrderInvoiceFiles).pipe(concat(cssOrderInvoiceConfig.minifiedFileName + ".css")).pipe(gulp.dest("./wwwroot/assets/styles/")));
//    return result;
//});

gulp.task("copy:dev", function () {
    var result = merge();
    result.add(gulp.src(["./app/**/*.*"]).pipe(gulp.dest("./wwwroot/app/")));
    result.add(gulp.src(["./temp/bootstrap/**/*.*" ]).pipe(gulp.dest("./wwwroot/lib/bootstrap/css")));
    result.add(gulp.src(["./temp/less/**/*.*"]).pipe(gulp.dest("./wwwroot/assets/styles/")));
    result.add(gulp.src(["./assets/images/**/*.*" ]).pipe(gulp.dest("./wwwroot/assets/images/")));
    result.add(gulp.src(["./assets/fonts/**/*.*" ]).pipe(gulp.dest("./wwwroot/lib/fonts/")));
    result.add(gulp.src(["./assets/templates/**/*.*" ]).pipe(gulp.dest("./wwwroot/assets/templates/")));
    result.add(gulp.src(["./assets/miscellaneous/**/*.*" ]).pipe(gulp.dest("./wwwroot/assets/miscellaneous/")));
    result.add(gulp.src(["./app/core/utils/ace/**/*.*" ]).pipe(gulp.dest("./wwwroot/lib/ace-builds/src-min-noconflict/")));
    result.add(gulp.src(["./wwwroot/lib/ace-builds/src-min-noconflict/worker-html.js", "./wwwroot/lib/ace-builds/src-min-noconflict/worker-css.js"]).pipe(gulp.dest("./temp/js/")));
    result.add(gulp.src(["./app/core/utils/ace/worker-ttl.js"]).pipe(gulp.dest("./temp/js/")));
    return result;
});

gulp.task("copy:rel", function () {
    var result = merge();
    result.add(gulp.src(["./assets/images/**/*.*", ]).pipe(gulp.dest("./wwwroot/assets/images/")));
    result.add(gulp.src(["./assets/fonts/**/*.*", ]).pipe(gulp.dest("./wwwroot/fonts/")));
    result.add(gulp.src(["./assets/miscellaneous/**/*.*", ]).pipe(gulp.dest("./wwwroot/assets/miscellaneous/")));
    result.add(gulp.src(["./assets/templates/**/*.*" ]).pipe(gulp.dest("./wwwroot/assets/templates/")));
    result.add(gulp.src(["./wwwroot/lib/bootstrap/fonts/**/*.*" ]).pipe(gulp.dest("./wwwroot/fonts/")));
    return result;
});

gulp.task("html2js", function() {
    return gulp.src('app/**/*.html')
        .pipe(htmlmin({
            empty: true,
            spare: true,
            quotes: true
        }))
        .pipe(html2js({
            moduleName: 'templates'
        }))
        .pipe(concat("templates.js"))
        .pipe(gulp.dest("./wwwroot/app/"));
});

gulp.task("less", function () {
    var result = merge();
    result.add(
        gulp.src([
        './Assets/styles/site.less',
        './Assets/styles/order-invoice.less'
        ]).pipe(less())
        .pipe(gulp.dest("./temp/less/"))
    );
    result.add(
        gulp.src([
        './Assets/styles/bootstrap/bootstrap.less'
        ]).pipe(less())
        .pipe(gulp.dest("./temp/bootstrap/"))
    );
    return result;
});

gulp.task("min:js", function () {
    var result = merge();
    result.add(
        gulp.src(jsFiles)
        //.pipe(print())
        .pipe(concat(jsConfig.minifiedFileName + ".min.js"))
        .pipe(uglify())
        .pipe(gulp.dest("./wwwroot/"))
    );
    result.add(
        gulp.src(['./temp/js/worker-ttl.js'])
        //.pipe(print())
        .pipe(concat("worker-ttl.js"))
        .pipe(uglify())
        .pipe(gulp.dest("./wwwroot/"))
    );
    result.add(
        gulp.src(['./temp/js/worker-css.js'])
        //.pipe(print())
        .pipe(concat("worker-css.js"))
        .pipe(uglify())
        .pipe(gulp.dest("./wwwroot/"))
    );
    result.add(
        gulp.src(['./temp/js/worker-html.js'])
        //.pipe(print())
        .pipe(concat("worker-html.js"))
        .pipe(uglify())
        .pipe(gulp.dest("./wwwroot/"))
    );
    return result;
});

gulp.task("min:css", function () {
    var result = merge();
    result.add(
        gulp.src(cssFiles)
        //.pipe(print())
        .pipe(concat(cssConfig.minifiedFileName + ".min.css"))
        .pipe(cssmin())
        .pipe(gulp.dest("./wwwroot/"))
    );
    result.add(
        gulp.src(cssOrderInvoiceFiles)
        //.pipe(print())
        .pipe(concat(cssOrderInvoiceConfig.minifiedFileName + ".min.css"))
        .pipe(cssmin())
        .pipe(gulp.dest("./wwwroot/"))
    );
    return result;
});

gulp.task("clean:js", function (cb) {
    rimraf("./wwwroot/*.js", cb);
});

gulp.task("clean:css", function (cb) {
    rimraf("./wwwroot/*.css", cb);
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