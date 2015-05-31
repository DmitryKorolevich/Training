﻿// This file in the main entry point for defining grunt tasks and using grunt plugins.
// Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409

module.exports = function (grunt) {
	var jsFiles = grunt.file.readJSON('AppConfig/scripts/files.json').files;
	var cssFiles = grunt.file.readJSON('AppConfig/styles/files.json').files;

	for (var i = 0; i < jsFiles.length; i++) {
		jsFiles[i] = "wwwroot/" + jsFiles[i];
	}

	grunt.initConfig({
		pkg: grunt.file.readJSON('package.json'),
        bower: {
		    install: {
			    options: {
				    targetDir: "wwwroot/lib",
				    layout: "byComponent",
				    cleanTargetDir: false
			    }
		    }
        },
        concat: {
        	options: {
        		// define a string to put between each file in the concatenated output
        		separator: ' '
        	},
			css: {
				src: ['temp/css/**/*.css'],
				dest: 'temp/css/site.css'
			},
			js: {
				src: jsFiles,
				// the location of the resulting JS file
				dest: 'temp/js/app.js'/*dist*/
			}
        },
        less: {
        	development: {
		        options: {
			        paths: ["assets/styles"],
		        },
		        files:
		        [
			        {
				        expand: true,
				        cwd: 'assets/styles/',
				        src: ['*.less', '!{boot,var,mix}*.less'],//src: ['**/*.less'
				        dest: 'temp/css/',
				        ext: '.css'
			        },
					{
						expand: true,
						cwd: 'assets/styles/bootstrap/',
						src: ['bootstrap.less'],
						dest: 'temp/boostrap/',
						ext: '.css'
					}
		        ]
	        }
        },
        uglify: {
        	options: {
        		// the banner is inserted at the top of the output
        		banner: '/*! <%= pkg.name %> <%= grunt.template.today("dd-mm-yyyy") %> */\n'
        	},
        	dist: {
        		files: {
        			'temp/js/minified/app.min.js': ['temp/js/**/*.js']
        		}
        	}
        },
        clean:{ 
        	wwwroot: ["wwwroot/app", "wwwroot/assets"],
			temp: ["temp"]
        },
        copy: {
        	development: {
        		files: [
				  { expand: true, cwd: 'app/', src: ['**'], dest: 'wwwroot/app/' },
				  { expand: true, cwd: 'temp/css/', src: ['**'], dest: 'wwwroot/assets/styles/' },
				  { expand: true, cwd: 'temp/boostrap/', src: ['**'], dest: 'wwwroot/lib/bootstrap/css/' },
				  { expand: true, cwd: 'assets/images/', src: ['**'], dest: 'wwwroot/assets/images/' },
				  { expand: true, cwd: 'assets/fonts/', src: ['**'], dest: 'wwwroot/lib/fonts/' },
				  { expand: true, cwd: 'assets/miscellaneous/', src: ['**'], dest: 'wwwroot/assets/miscellaneous/' },
                  { expand: true, cwd: 'app/core/utils/ace/', src: ['**'], dest: 'wwwroot/lib/ace-builds/src-min-noconflict/' }
        		]
        	},
        	release: {
		        files: [
			        { expand: true, cwd: 'temp/js/', src: ['**'], dest: 'wwwroot/app/' },
			        { expand: true, cwd: 'temp/js/minified/', src: ['**'], dest: 'wwwroot/app/' },
			        { expand: true, cwd: 'temp/css/minified/', src: ['**'], dest: 'wwwroot/assets/styles/' },
					{ expand: true, cwd: 'assets/images/', src: ['**'], dest: 'wwwroot/assets/images/' },
					{ expand: true, cwd: 'assets/miscellaneous/', src: ['**'], dest: 'wwwroot/assets/miscellaneous/' },
					{ expand: true, cwd: 'app/core/utils/ace/', src: ['**'], dest: 'wwwroot/lib/ace-builds/src-min-noconflict/' }
		        ]
	        }
        },
        cssmin: {
        	target: {
        		files: [
					{ expand: true, cwd: 'temp/css/', src: ['site.css'], dest: 'temp/css/minified/', ext: '.min.css' },
					{ expand: true, cwd: 'temp/boostrap/', src: ['boostrap.css'], dest: 'temp/boostrap/minified/', ext: '.min.css' },
        		],
        		options: {
        			shorthandCompacting: false,
        			roundingPrecision: -1
        		}
        	}
        },
		html2js: {
			options: {
				base: '',
				module: 'templates',
				singleModule: true,
				useStrict: true,
				htmlmin: {
					collapseBooleanAttributes: true,
					collapseWhitespace: true,
					removeAttributeQuotes: true,
					removeComments: true,
					removeEmptyAttributes: true,
					removeRedundantAttributes: true,
					removeScriptTypeAttributes: true,
					removeStyleLinkTypeAttributes: true
				}
			},
			main: {
				src: ['app/**/*.html'],
				dest: 'wwwroot/app/templates.js'
			}
		},
        watch: {
        	files: ['app/**/*.js', 'app/**/*.html', 'assets/**/*.less'],
        	tasks: ['development'/*, 'test'*/],
        	options: {
        		livereload: true
        	}
        }
    });

    // This command registers the default task which will install bower packages into wwwroot/lib
	grunt.registerTask("default", ["bower:install"]);

	grunt.registerTask("bower-install", ["bower:install"]);

	// this would be run by typing "grunt test" on the command line
	grunt.registerTask('test', ['jshint']);

	// the default task can be run just by typing "grunt" on the command line
	grunt.registerTask('development', ['clean:wwwroot', 'less', 'copy:development', 'clean:temp', 'html2js:main']);

	grunt.registerTask('release', ['clean:wwwroot', 'less', 'copy:development', 'concat', 'clean:wwwroot', 'cssmin', 'copy:release', 'clean:temp', 'html2js:main']);/*,'uglify'*/

	grunt.registerTask('regularWatch', ['watch']);

    // The following line loads the grunt plugins.
    // This line needs to be at the end of this this file.
    grunt.loadNpmTasks("grunt-bower-task");
	grunt.loadNpmTasks("grunt-contrib-less");
	grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-html2js');
};