// This file in the main entry point for defining grunt tasks and using grunt plugins.
// Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409

module.exports = function (grunt) {
	grunt.initConfig({
		pkg:grunt.file.readJSON('package.json'),
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
        		separator: ';'
        	},
			css: {
				
			},
			js: {
				src: ['App/**/*.js'],
				// the location of the resulting JS file
				dest: 'wwwroot/lib/<%= pkg.name %>.js'/*dist*/
			}
        },
        less: {
        	development: {
        		options: {
        			paths: ["Assets/styles"]
        		},
        		files: { "wwwroot/css/site.css": "assets/styles/*.less" }
        	}
        },
        uglify: {
        	options: {
        		// the banner is inserted at the top of the output
        		banner: '/*! <%= pkg.name %> <%= grunt.template.today("dd-mm-yyyy") %> */\n'
        	},
        	dist: {
        		files: {
        			'dist/<%= pkg.name %>.min.js': ['<%= concat.dist.dest %>']
        		}
        	}
        },
        jshint: {
        	// define the files to lint
        	files: ['gruntfile.js', 'src/**/*.js', 'test/**/*.js'],
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
        watch: {
        	files: ['<%= jshint.files %>'],
        	tasks: ['jshint']
        }
    });

    // This command registers the default task which will install bower packages into wwwroot/lib
	grunt.registerTask("default", ["bower:install"]);

	// this would be run by typing "grunt test" on the command line
	grunt.registerTask('test', ['jshint']);

	// the default task can be run just by typing "grunt" on the command line
	grunt.registerTask('default', ['jshint', 'concat', 'uglify']);

    // The following line loads the grunt plugins.
    // This line needs to be at the end of this this file.
    grunt.loadNpmTasks("grunt-bower-task");
	grunt.loadNpmTasks("grunt-contrib-less");
	grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-jshint');
    grunt.loadNpmTasks('grunt-contrib-qunit');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-concat');
};