


var app = angular.module('MazeProject', ['ngRoute', 'angularSpinner'])
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
                            templateUrl: 'content/scores.html',
                            controller: 'scoreCtrl'
						})
						.when("/single-mode", {
							templateUrl: "content/single.html",
							controller: 'singleCtrl'
						})
                        .when("/multi-mode", {
                            templateUrl: 'content/multi.html',
                            controller: 'multiCtrl'
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
                .controller('multiCtrl', function ($scope, $http) {
                    // Ajax call to have the scores



                })
                .controller('scoreCtrl', function ($scope, $http) {
                    // Ajax call to have the scores

                    $http({
                        method: 'GET',
                        url: ''
                    }).then(function (response) {
                        console.log(response);
                    }, function (error) {
                        console.log(error);
                        });



                })
				.controller('loginCtrl', function($scope){
					$scope.username = '';
					$scope.password = '';
					
				})
				.controller('homeCtrl', function($scope) {
				
				})
				.controller('aboutCtrl', function($scope) {

				})
                .controller('singleCtrl', function ($scope, $http, usSpinnerService) {
                    
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
                    $scope.algorithm = {
                        value: undefined,
                        errors: []
                    };
                    $scope.solve = true;
                    
					// validate

                    $scope.startGame = function () {
                        usSpinnerService.spin('spinner');
						var data = {
							name: $scope.name.value,
							rows: $scope.rows.value,
                            cols: $scope.cols.value,
						};
					// Ajax call to GenerateMaze
                        $http({
                            method: 'GET',
                            url: 'api/Default/5',
                            data: data
                        })
                        .then(function (response) {
                            console.log(response);
                            usSpinnerService.stop('spinner');
                            $scope.maze = {
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
                            $scope.maze.getValue = function (row, col) {
                                return this.mazeData[row * this.cols + col];
                            };
                            }, function (error) {
                                console.log(error);
                                usSpinnerService.stop('spinner');
                            });

                        $scope.$watch('algorithm.value', function (newVal, oldVal) {
                            if ((newVal == '0' || newVal == '1') && $scope.maze != undefined) {
                                $scope.solve = false;
                            } else {
                                $scope.solve = true;
                            }
                        });

					}

                    $scope.solveGame = function () {
                        $scope.solve = true;
                        var data = {
                            algorithm: $scope.algorithm.value
                        };
                        // http request
                        $http({
                            method: 'GET',
                            url: '',
                            data: data
                        })
                        .then(function (response) {

                            $scope.solution = "21112";
                            console.log($scope.solution);
                        }, function (error) {
                            console.log(error);
                            $scope.solve = false;
                        });
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
                        
                        $http({
                            method: 'GET',
                            url: '/api/values/3',
                            data: data
						})
						.then(function(response) {
							console.log(response);
                            }, function (error) {
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
        							
        								// Ajax call to validate user existance
        							
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
                })
                .directive('uaMaze', function ($document, $interval) {
                    return {
                        restrict: "A",
                        link: function (scope, element) {
                            var ctx;
                            var rows, cols;
                            var cellWidth, cellHeight;

                            function moveOneStep(direction) {
                                var maze = scope.maze;
                                var src = scope.maze.pos;
                                var dest = undefined;

                                if (direction == "ArrowUp" && maze.pos.row > 0) {
                                    dest = { row: maze.pos.row - 1, col: maze.pos.col };
                                } else if (direction == "ArrowDown" && maze.pos.row < maze.rows - 1) {
                                    dest = { row: maze.pos.row + 1, col: maze.pos.col };
                                } else if (direction == "ArrowRight" && maze.pos.col < maze.cols - 1) {
                                    dest = { row: maze.pos.row, col: maze.pos.col + 1 };
                                } else if (direction == "ArrowLeft" && maze.pos.col > 0) {
                                    dest = { row: maze.pos.row, col: maze.pos.col - 1 };
                                }

                                if (dest != undefined && maze.getValue(dest.row, dest.col) != '1') {

                                    ctx.clearRect(src.col * cellWidth, src.row * cellHeight, cellWidth, cellHeight);
                                    var img3 = new Image();

                                    var x = cellWidth * dest.col;
                                    var y = cellHeight * dest.row;

                                    img3.onload = function () {
                                        ctx.drawImage(img3, x, y, cellWidth, cellHeight);
                                        maze.pos.row = dest.row;
                                        maze.pos.col = dest.col;
                                    };
                                    img3.src = "resources/player.png";

                                    if (maze.getValue(dest.row, dest.col) == '#') {
                                        onWin();
                                    }

                                }
                            }
                            function onWin() {
                                $document.unbind('keydown');
                                alert("You win!");
                            }

                            scope.$watch('maze', function (newVal, oldVal) {
                                if (newVal != undefined) {
                                    console.log(scope);
                                    ctx = element[0].getContext('2d');
                                    rows = newVal.rows;
                                    cols = newVal.cols;
                                    cellWidth = element[0].width / cols;
                                    cellHeight = element[0].height / rows;


                                    for (var i = 0; i < rows; i++)
                                        for (var j = 0; j < cols; j++) {
                                            var val = scope.maze.mazeData[(i * cols) + j];
                                            switch (val) {
                                                case '1':
                                                    ctx.fillRect(cellWidth * j, cellHeight * i, cellWidth, cellHeight);
                                                    break;
                                                case '#':
                                                    var img1 = new Image();

                                                    var x1 = cellWidth * j;
                                                    var y1 = cellHeight * i;

                                                    img1.onload = function () {
                                                        ctx.drawImage(img1, x1, y1, cellWidth, cellHeight);
                                                    };
                                                    img1.src = "resources/end.png";
                                                    break;
                                                case '*':

                                                    var img2 = new Image();
                                                    var x2 = cellWidth * j;
                                                    var y2 = cellHeight * i;

                                                    img2.onload = function () {
                                                        ctx.drawImage(img2, x2, y2, cellWidth, cellHeight);
                                                    };
                                                    img2.src = "resources/player.png";

                                                    break;
                                                default:
                                                    console.log('Hi');
                                                    break;
                                            }
                                        }

                                    $document.bind('keydown', function (e) {
                                        moveOneStep(e.key);
                                    });



                                }
                            });
                            scope.$watch('solution', function (newVal, oldVal) {
                                if (newVal != undefined) {
                                    var i = 0;
                                    var stop = $interval(function () {
                                        if (newVal[i] == 0)
                                            moveOneStep("ArrowLeft");
                                        else if (newVal[i] == 1)
                                            moveOneStep("ArrowRight");
                                        else if (newVal[i] == 2)
                                            moveOneStep("ArrowUp");
                                        else if (newVal[i] == 3)
                                            moveOneStep("ArrowDown");
                                        i++;
                                        if (i >= newVal.length)
                                            $interval.cancel(stop);
                                    }, 300);
                                }
                            });

                        }
                    };
                });