

var JQ = jQuery.noConflict();

var app = angular.module('MazeProject', ['ngRoute'])
				.config(function($routeProvider, $locationProvider) {
					$routeProvider
						.when("/", {

						})
						.when("/register", {
							templateUrl: 'content/register.html',
							controller: 'regCtrl'
						})
						.when("/home", {
							templateUrl: 'content/home.html',
							controller: 'homeCtrl'
						})
						.when("/about", {
							templateUrl: 'content/about.html',
							controller: 'aboutCtrl'
						})
						.when("/scores", {

						})
						.when("/single-mode", {
							templateUrl: "content/single.html",
							controller: 'singleCtrl'
						})
						.when("/multi-mode", {

						})
						.when("/settings", {

						})
						.when("/login", {
							templateUrl: 'content/login.html',
							contoller: 'loginCtrl'
                        })
						.otherwise({redirectTo: '/'});
						$locationProvider.html5Mode(true);
				})
				.controller('loginCtrl', function($scope){
					$scope.username = '';
					$scope.password = '';
					
				})
				.controller('homeCtrl', function($scope) {
				
				})
				.controller('aboutCtrl', function($scope) {

				})
				.controller('singleCtrl', function($scope) {
                    $scope.name = {
                        value: '',
                        errors: []
                    };
                    $scope.rows = {
                        value: '',
                        errors: []
                    };
                    $scope.cols = {
                        value: '',
                        errors: []
                    };
					$scope.algorithm = '';

                    $scope.maze = undefined;

					// validate

					$scope.startGame = function() {
						var data = {
							name: $scope.name,
							rows: $scope.rows,
							cols: $scope.cols
						};
						
					// Ajax call to GenerateMaze
						JQ.get({
							url: 'api/Default/5',
                        })
                        .done(function (response) {
                            console.log(response);
						})
						.fail(function (error) {
							alert('An error occured!');
						});
					
                        var maze = {
                            mazeData: "01101#00000001*000100100011001",
                            rows: 5,
                            cols: 6,
                            pos: {
                                row: 2,
                                col: 2
                            },
                            startPos: {
                                row: 2,
                                col: 2
                            },
                            endPos: {
                                row: 0,
                                col: 5
                            }
                        };
                        maze.getValue = function (row, col) {
                            return this.mazeData[row * this.cols + col];
                        };
						JQ("#game").mazeBoard(maze);

					}

                    $scope.solveGame = function () {
                        
                    }


				})
				.controller('regCtrl', function($scope) {
					var details = {
						username: {
							value: '',
							regex: /^[a-zA-Z][a-zA-Z0-9_]+$/,
							errors: {length: '', reg: '', unique: ''}
						},
						password: {
							value: '',
							regex: /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*/,
							errors: {length: '', reg: ''}
						},
						passvalid: {
							value: '',
							regex: '',
							errors: []
						},
						email: {
							value: '',
							regex: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/,
							errors: []
						}
					};


					$scope.details = details;

					// validation
					$scope.register = function() {
						var data = {
							username: $scope.details.username.value,
							password: $scope.details.password.value,
							email: $scope.details.email.value
						};
						// ajax call with details

						JQ.get({
                            url: '/api/values/3',
                            data: data
						})
						.done(function(response) {
							console.log(response);
						})
						.fail(function(error) {
							console.log(error);
						});
					}
				})
				.directive('uaUsername', function() {
 					return {
    					require: 'ngModel',
    					link: function(scope, element, attr, mCtrl) {
      							function userValidation(value) {
        							if (value.length >= 6 && value.length < 26) {
        								scope.details.username.errors.length = '';
          								mCtrl.$setValidity('length', true);
        							} else {
        								if (scope.details.username.errors.length == '')
        									scope.details.username.errors.length = "Username must be 6-25 characters long.";
          								mCtrl.$setValidity('length', false);
        							}
        							if (scope.details.username.regex.test(value)) {
        								scope.details.username.errors.reg = '';
        								mCtrl.$setValidity('regex', true);
        							} else {
        								if (scope.details.username.errors.reg == '')
        									scope.details.username.errors.reg = "Username must start with a letter, and contain only alpha-numerical characters or semi-collon.";
        								mCtrl.$setValidity('regex', false);
        							}
        							if (scope.details.username.errors.length == '' && scope.details.username.errors.reg == '') {
        								// Ajax call to validate user existance
        							}
        							return value;
      							}
      							mCtrl.$parsers.push(userValidation);
    					}
  					};
				})
				.directive('uaPassword', function() {
 					return {
    					require: 'ngModel',
    					link: function(scope, element, attr, mCtrl) {
      							function passValidation(value) {
        							if (value.length >= 8 && value.length < 23) {
        								scope.details.password.errors.length = '';
          								mCtrl.$setValidity('length', true);
        							} else {
        								if (scope.details.password.errors.length == '')
        									scope.details.password.errors.length = "Password must be 8-22 characters long.";
        								mCtrl.$setValidity('length', false);
        							}
        							if (scope.details.password.regex.test(value)) {
        								scope.details.password.errors.reg = '';
        								mCtrl.$setValidity('regex', true);
        							} else {
        								if (scope.details.password.errors.reg == '')
        									scope.details.password.errors.reg = "Password must contain at least 1 upper-case letter, 1 lower-case letter and 1 numerical character.";
        								mCtrl.$setValidity('regex', false);
        							}
        							return value;
      							}
      							mCtrl.$parsers.push(passValidation);
    					}
  					};
				})
				.directive('uaValid', function() {
 					return {
    					require: 'ngModel',
    					link: function(scope, element, attr, mCtrl) {
      							function passvalidValidation(value) {
        							if (scope.details.password.value == value) {
        								scope.details.passvalid.errors = [];
          								mCtrl.$setValidity('equal', true);
        							} else {
        								if (scope.details.passvalid.errors.length == 0)
        									scope.details.passvalid.errors.push("Password confirmation must be equal to the password.");
          								mCtrl.$setValidity('equal', false);
        							}
        							return value;
      							}
      							mCtrl.$parsers.push(passvalidValidation);
    					}
  					};
				})
				.directive('uaEmail', function() {
 					return {
    					require: 'ngModel',
    					link: function(scope, element, attr, mCtrl) {
      							function emailValidation(value) {
        							if (scope.details.email.regex.test(value)) {
        								scope.details.email.errors = [];
          								mCtrl.$setValidity('valid', true);
        							} else {
        								if (scope.details.email.errors.length == 0)
        									scope.details.email.errors.push("E-Mail should be in a valid format.");
          								mCtrl.$setValidity('valid', false);
        							}
        							return value;
      							}
      							mCtrl.$parsers.push(emailValidation);
    					}
  					};
				})
                .directive('uaGame', function() {
 					return {
    					require: 'ngModel',
    					link: function(scope, element, attr, mCtrl) {
                            function gameValidation(value) {
                                var regex = /^[a-zA-Z]+$/;
        							if (regex.test(value)) {
        								scope.name.errors = [];
          								mCtrl.$setValidity('letters', true);
        							} else {
        								if (scope.name.errors.length == 0)
                                            scope.name.errors.push("Game's name should consist of letters only.");
          								mCtrl.$setValidity('letters', false);
        							}
        							return value;
      							}
      							mCtrl.$parsers.push(gameValidation);
    					}
  					};
    })
                .directive('uaRows', function() {
 					return {
    					require: 'ngModel',
    					link: function(scope, element, attr, mCtrl) {
                            function rowsValidation(value) {
                                var regex = /^\d+/;
        							if (regex.test(value)) {
        								scope.rows.errors = [];
                                        mCtrl.$setValidity('numbers', true);
                                        if (value > 0) {
                                            scope.rows.errors = [];
                                            mCtrl.$setValidity('illegal', true);
                                        } else {
                                            if (scope.rows.errors.length == 0)
                                                scope.rows.errors.push("Rows should be bigger than zero.");
                                            mCtrl.$setValidity('illegal', false);
                                        }
        							} else {
        								if (scope.rows.errors.length == 0)
                                            scope.rows.errors.push("Rows should consist of numbers only.");
          								mCtrl.$setValidity('numbers', false);
                                    }
        							return value;
      							}
      							mCtrl.$parsers.push(rowsValidation);
    					}
  					};
    })
                .directive('uaCols', function() {
 					return {
    					require: 'ngModel',
    					link: function(scope, element, attr, mCtrl) {
                            function colsValidation(value) {
                                var regex = /^\d+/;
        							if (regex.test(value)) {
        								scope.cols.errors = [];
                                        mCtrl.$setValidity('numbers', true);
                                        if (value > 0) {
                                            scope.cols.errors = [];
                                            mCtrl.$setValidity('illegal', true);
                                        } else {
                                            if (scope.cols.errors.length == 0)
                                                scope.cols.errors.push("Cols should be bigger than zero.");
                                            mCtrl.$setValidity('illegal', false);
                                        }
        							} else {
                                        if (scope.cols.errors.length == 0)
                                            scope.cols.errors.push("Cols should consist of numbers only.");
          								mCtrl.$setValidity('numbers', false);
                                    }
        							return value;
      							}
      							mCtrl.$parsers.push(colsValidation);
    					}
  					};
				});
