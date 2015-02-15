// This file in the main entry point for defining grunt tasks and using grunt plugins.
// Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409

module.exports = function (grunt) {
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
				src: ['App/**/*.js'],
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
        			'temp/js/minified/app.min-<%= grunt.template.today("dd-mm-yyyy") %>.js': ['temp/js/**/*.js']
        		}
        	}
        },
        jshint: {
        	// define the files to lint
        	files: ['app/**/*.js'],
        	// configure JSHint (documented at http://www.jshint.com/docs/)
        	options: {
        		// more options here if you want to override JSHint defaults
        		globals: {
        			jQuery: true,
        			console: true,
        			module: true
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
				  { expand: true, cwd: 'assets/images/', src: ['**'], dest: 'wwwroot/assets/images/' }
        		]
        	},
        	release: {
		        files: [
			        { expand: true, cwd: 'temp/js/minified/', src: ['**'], dest: 'wwwroot/app/' },
			        { expand: true, cwd: 'temp/css/minified/', src: ['**'], dest: 'wwwroot/assets/styles/' },
					{ expand: true, cwd: 'temp/boostrap/minified/', src: ['**'], dest: 'wwwroot/lib/bootstrap/css/' },
					{ expand: true, cwd: 'assets/images/', src: ['**'], dest: 'wwwroot/assets/images/' }
		        ]
	        }
        },
        cssmin: {
        	target: {
        		files: [
					{ expand: true, cwd: 'temp/css/', src: ['site.css'], dest: 'temp/css/minified/', ext: '.min-<%= grunt.template.today("dd-mm-yyyy") %>.css' },
					{ expand: true, cwd: 'temp/boostrap/', src: ['boostrap.css'], dest: 'temp/boostrap/minified/', ext: '.min.css' },
        		],
        		options: {
        			shorthandCompacting: false,
        			roundingPrecision: -1
        		}
        	}
        },
        watch: {
        	files: ['app/**/*.js', 'app/**/*.html', 'assets/**/*.less'],
        	tasks: ['development'/*, 'test'*/],
        	options: {
        		livereload: true,
        	},
        }
    });

    // This command registers the default task which will install bower packages into wwwroot/lib
	grunt.registerTask("default", ["bower:install", "watch"]);

	// this would be run by typing "grunt test" on the command line
	grunt.registerTask('test', ['jshint']);

	// the default task can be run just by typing "grunt" on the command line
	grunt.registerTask('development', ['clean:wwwroot', 'less', 'copy:development', 'clean:temp']);

	grunt.registerTask('release', ['clean:wwwroot', 'less', 'jshint', 'concat', 'uglify', 'cssmin', 'copy:release', 'clean:temp']);

	grunt.registerTask('regularWatch', ['watch']);

    // The following line loads the grunt plugins.
    // This line needs to be at the end of this this file.
    grunt.loadNpmTasks("grunt-bower-task");
	grunt.loadNpmTasks("grunt-contrib-less");
	grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-jshint');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
};