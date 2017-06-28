


var app = angular.module('MazeProject', ['ngRoute', 'angularSpinner'])
    .config(function ($routeProvider, $locationProvider) {
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
            .otherwise({ redirectTo: '/' });
        $locationProvider.html5Mode(true);
    })
    .controller('multiCtrl', function ($scope, $http, $timeout) {
        // Ajax call to have the scores

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

        $scope.maze = undefined;
        $scope.game = undefined;
        $scope.games = [];
        $scope.refresh = false;
        $scope.id = undefined;
        $scope.opp_id = undefined;

        $scope.movesHub = $.connection.movesHub;

        $http({
            method: 'GET',
            url: 'api/Multiplayer/GetList'
        }).then(function (response) {
            $scope.games = response.data;
        }, function (error) {
            console.log(error);
        });

        $scope.refreshList = function () {
            // Delay the timeout
            $scope.refresh = true;
            $timeout(function () {
                $scope.refresh = false;
            }, 3000);

            $http({
                method: 'GET',
                url: 'api/Multiplayer/GetList'
            }).then(function (response) {
                $scope.games = response.data;
            }, function (error) {
                console.log(error);
            });
        }

        $scope.hostGame = function () {
            var data = {
                name: $scope.name.value,
                rows: $scope.rows.value,
                cols: $scope.cols.value,
                sessionToken: '1'
            };

            $http({
                method: 'POST',
                url: '/api/Multiplayer/StartNewGame',
                data: data
            })
                .then(function (response) {
                    $scope.maze = response.data;
                    $scope.maze.getValue = function (row, col) {
                        return this.Maze[row * this.Cols + col];
                    };
                    $.connection.hub.start().then(function () {
                        $scope.id = $scope.maze.Player1Id;
                        $scope.opp_id = $scope.maze.Player2Id;
                        console.log($scope.maze);
                        $scope.movesHub.server.connect($scope.maze.Player1Id);
                        console.log("connected!");
                    });
                }, function (error) {
                    console.log(error);
                });

        }

        $scope.joinGame = function () {

            var data = {
                name: $scope.game,
                sessionToken: '2'
            };

            $http({
                method: 'POST',
                url: 'api/Multiplayer/JoinGame',
                data: data
            })
                .then(function (response) {
                    $scope.maze = response.data;
                    $scope.maze.getValue = function (row, col) {
                        return this.Maze[row * this.Cols + col];
                    };
                    $.connection.hub.start().then(function () {
                        $scope.id = $scope.maze.Player2Id;
                        $scope.opp_id = $scope.maze.Player1Id;
                        $scope.movesHub.server.connect($scope.maze.Player2Id);
                    });
                }, function (error) {
                    console.log(error);
                });
        }


    })
    .controller('scoreCtrl', function ($scope, $http) {

        $scope.amount = 5;
        $scope.records = [];

        // Ajax call to have the scores
        $http({
            method: 'GET',
            url: 'api/Users/Records/' + $scope.amount,
        }).then(function (response) {
            console.log(response);
            $scope.records = response.data;
        }, function (error) {
            console.log(error);
            });

        
        
    })
    .controller('loginCtrl', function ($scope) {
        $scope.username = '';
        $scope.password = '';

    })
    .controller('homeCtrl', function ($scope) {

    })
    .controller('aboutCtrl', function ($scope) {

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
        $scope.solve_clicked = false;
        // validate

        $scope.startGame = function () {
            $scope.solve_clicked = false;
            usSpinnerService.spin('spinner');
            var data = {
                name: $scope.name.value,
                rows: $scope.rows.value,
                cols: $scope.cols.value,
            };
            // Ajax call to GenerateMaze
            $http({
                method: 'POST',
                url: 'api/Singleplayer/CreateGame',
                data: data
            })
                .then(function (response) {
                    usSpinnerService.stop('spinner');
                    $scope.maze = response.data;
                    $scope.maze.getValue = function (row, col) {
                        return this.Maze[row * this.Cols + col];
                    };
                }, function (error) {
                    console.log(error);
                    usSpinnerService.stop('spinner');
                });

            $scope.$watch('algorithm.value', function (newVal, oldVal) {
                if ((newVal == '0' || newVal == '1') && $scope.maze != undefined && !$scope.solve_clicked) {
                    $scope.solve = false;
                    $scope.solve_clicked = true;
                } else {
                    $scope.solve = true;
                }
            });

        }

        $scope.solveGame = function () {
            $scope.solve = true;
            var data = {
                game: $scope.maze,
                algorithm: $scope.algorithm.value
            };
            // http request
            $http({
                method: 'POST',
                url: 'api/Singleplayer/Solve',
                data: data
            })
                .then(function (response) {
                    $scope.solution = response.data;
                    console.log($scope.solution);
                }, function (error) {
                    console.log(error);
                    $scope.solve = false;
                });
        }
    })
    .controller('regCtrl', function ($scope) {
        var details = {
            username: {
                value: '',
                regex: /^[a-zA-Z][a-zA-Z0-9_]+$/,
                errors: { length: '', reg: '', unique: '' }
            },
            password: {
                value: '',
                regex: /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*/,
                errors: { length: '', reg: '' }
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
        $scope.register = function () {
            var data = {
                username: $scope.details.username.value,
                password: $scope.details.password.value,
                email: $scope.details.email.value
            };
            // ajax call with details

            $http({
                method: 'POST',
                url: 'api/Users/Register',
                data: data
            })
                .then(function (response) {
                    console.log(response);
                }, function (error) {
                    console.log(error);
                });

        }
    })
    .directive('uaUsername', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attr, mCtrl) {
                function userValidation(value) {
                    if (value.length >= 6 && value.length < 26) {
                        scope.details.username.errors.length = '';
                        mCtrl.$setValidity('length', true);
                    } else {
                        if (scope.details.username.errors.length === '')
                            scope.details.username.errors.length = "Username must be 6-25 characters long.";
                        mCtrl.$setValidity('length', false);
                    }
                    if (scope.details.username.regex.test(value)) {
                        scope.details.username.errors.reg = '';
                        mCtrl.$setValidity('regex', true);
                    } else {
                        if (scope.details.username.errors.reg === '')
                            scope.details.username.errors.reg = "Username must start with a letter, and contain only alpha-numerical characters or semi-collon.";
                        mCtrl.$setValidity('regex', false);
                    }


                    $http({
                        method: 'GET',
                        url: 'api/Users/ValidateUsername',
                        data: value
                    }).then(function (response) {
                        if (response) {
                            scope.details.username.errors.unique = '';
                            mCtrl.$setValidity('unique', true);
                        } else {
                            if (scope.details.username.errors.unique === '')
                                scope.details.username.errors.unique = "Username already exists.";
                            mCtrl.$setValidity('unique', false);
                        }
                    }, function (error) {
                        console.log(error);
                    });


                    // Ajax call to validate user existance

                    return value;
                }
                mCtrl.$parsers.push(userValidation);
            }
        };
    })
    .directive('uaPassword', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attr, mCtrl) {
                function passValidation(value) {
                    if (value.length >= 8 && value.length < 23) {
                        scope.details.password.errors.length = '';
                        mCtrl.$setValidity('length', true);
                    } else {
                        if (scope.details.password.errors.length === '')
                            scope.details.password.errors.length = "Password must be 8-22 characters long.";
                        mCtrl.$setValidity('length', false);
                    }
                    if (scope.details.password.regex.test(value)) {
                        scope.details.password.errors.reg = '';
                        mCtrl.$setValidity('regex', true);
                    } else {
                        if (scope.details.password.errors.reg === '')
                            scope.details.password.errors.reg = "Password must contain at least 1 upper-case letter, 1 lower-case letter and 1 numerical character.";
                        mCtrl.$setValidity('regex', false);
                    }
                    return value;
                }
                mCtrl.$parsers.push(passValidation);
            }
        };
    })
    .directive('uaValid', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attr, mCtrl) {
                function passvalidValidation(value) {
                    if (scope.details.password.value === value) {
                        scope.details.passvalid.errors = [];
                        mCtrl.$setValidity('equal', true);
                    } else {
                        if (scope.details.passvalid.errors.length === 0)
                            scope.details.passvalid.errors.push("Password confirmation must be equal to the password.");
                        mCtrl.$setValidity('equal', false);
                    }
                    return value;
                }
                mCtrl.$parsers.push(passvalidValidation);
            }
        };
    })
    .directive('uaEmail', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attr, mCtrl) {
                function emailValidation(value) {
                    if (scope.details.email.regex.test(value)) {
                        scope.details.email.errors = [];
                        mCtrl.$setValidity('valid', true);
                    } else {
                        if (scope.details.email.errors.length === 0)
                            scope.details.email.errors.push("E-Mail should be in a valid format.");
                        mCtrl.$setValidity('valid', false);
                    }
                    return value;
                }
                mCtrl.$parsers.push(emailValidation);
            }
        };
    })
    .directive('uaGame', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attr, mCtrl) {
                function gameValidation(value) {
                    var regex = /^[a-zA-Z]+$/;
                    if (regex.test(value)) {
                        scope.name.errors = [];
                        mCtrl.$setValidity('letters', true);
                    } else {
                        if (scope.name.errors.length === 0)
                            scope.name.errors.push("Game's name should consist of letters only.");
                        mCtrl.$setValidity('letters', false);
                    }
                    return value;
                }
                mCtrl.$parsers.push(gameValidation);
            }
        };
    })
    .directive('uaRows', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attr, mCtrl) {
                function rowsValidation(value) {
                    var regex = /^\d+/;
                    if (regex.test(value)) {
                        scope.rows.errors = [];
                        mCtrl.$setValidity('numbers', true);
                        if (value > 0) {
                            scope.rows.errors = [];
                            mCtrl.$setValidity('illegal', true);
                        } else {
                            if (scope.rows.errors.length === 0)
                                scope.rows.errors.push("Rows should be bigger than zero.");
                            mCtrl.$setValidity('illegal', false);
                        }
                    } else {
                        if (scope.rows.errors.length === 0)
                            scope.rows.errors.push("Rows should consist of numbers only.");
                        mCtrl.$setValidity('numbers', false);
                    }
                    return value;
                }
                mCtrl.$parsers.push(rowsValidation);
            }
        };
    })
    .directive('uaCols', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attr, mCtrl) {
                function colsValidation(value) {
                    var regex = /^\d+/;
                    if (regex.test(value)) {
                        scope.cols.errors = [];
                        mCtrl.$setValidity('numbers', true);
                        if (value > 0) {
                            scope.cols.errors = [];
                            mCtrl.$setValidity('illegal', true);
                        } else {
                            if (scope.cols.errors.length === 0)
                                scope.cols.errors.push("Cols should be bigger than zero.");
                            mCtrl.$setValidity('illegal', false);
                        }
                    } else {
                        if (scope.cols.errors.length === 0)
                            scope.cols.errors.push("Cols should consist of numbers only.");
                        mCtrl.$setValidity('numbers', false);
                    }
                    return value;
                }
                mCtrl.$parsers.push(colsValidation);
            }
        };
    })
    .directive('uaMaze', function ($document, $interval, $http) {
        return {
            restrict: "A",
            link: function (scope, element, attrs) {
                if (attrs.uaMaze == "rival") {
                    scope.movesHub.client.gotMove = function (playerId, key) {
                        moveOneStep(key);
                        if (scope.maze.OppPos.Row == scope.maze.End.Row &&
                            scope.maze.OppPos.Col == scope.maze.End.Col)
                            onLose();
                    };
                }
                var ctx;
                var rows, cols;
                var cellWidth, cellHeight;

                function moveOneStep(direction) {
                    var maze = scope.maze;
                    var src = undefined;

                    if (attrs.uaMaze == "rival") {
                        src = scope.maze.OppPos;
                    } else {
                        src = scope.maze.CurrentPos;
                    }

                    var dest = undefined;

                    if (direction === "ArrowUp" && src.Row > 0) {
                        dest = { row: src.Row - 1, col: src.Col };
                    } else if (direction === "ArrowDown" && src.Row < maze.Rows - 1) {
                        dest = { row: src.Row + 1, col: src.Col };
                    } else if (direction === "ArrowRight" && src.Col < maze.Cols - 1) {
                        dest = { row: src.Row, col: src.Col + 1 };
                    } else if (direction === "ArrowLeft" && src.Col > 0) {
                        dest = { row: src.Row, col: src.Col - 1 };
                    }

                    if (dest !== undefined && maze.getValue(dest.row, dest.col) !== '1') {
                        ctx.clearRect(src.Col * cellWidth, src.Row * cellHeight, cellWidth, cellHeight);
                        var img3 = new Image();

                        var x = cellWidth * dest.col;
                        var y = cellHeight * dest.row;

                        img3.onload = function () {
                            ctx.drawImage(img3, x, y, cellWidth, cellHeight);
                        };
                        img3.src = "resources/player.png";

                        if (attrs.uaMaze == "rival") {
                            maze.OppPos.Row = dest.row;
                            maze.OppPos.Col = dest.col;
                        } else {
                            maze.CurrentPos.Row = dest.row;
                            maze.CurrentPos.Col = dest.col;
                        }

                    }
                }
                function onWin() {
                    console.log("You win!");
                    $document.unbind('keydown');
          
                    // Update scores at server
                    $http({
                        method: 'GET',
                        url: 'api/Multiplayer/PlayerWon/1'
                    }).then(function (response) {
                        console.log("Done");
                        }, function (error) {
                            console.log("Error");
                        });
                }
                function onLose() {
                    console.log("You lost!");
                    $document.unbind('keydown');
                }

                scope.$watch('maze', function (newVal, oldVal) {
                    console.log(newVal);
                    if (newVal !== undefined) {
                        ctx = element[0].getContext('2d');
                        rows = newVal.Rows;
                        cols = newVal.Cols;
                        cellWidth = element[0].width / cols;
                        cellHeight = element[0].height / rows;


                        for (var i = 0; i < rows; i++)
                            for (var j = 0; j < cols; j++) {
                                var val = scope.maze.Maze[(i * cols) + j];
                                switch (val) {
                                    case '0':
                                        ctx.clearRect(cellWidth * j, cellHeight * i, cellWidth, cellHeight);
                                        break;
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
                                        break;
                                }
                            }

                        if (attrs.uaMaze != "rival") {
                            $document.bind('keydown', function (e) {
                                moveOneStep(e.key);
                                console.log(scope.id, scope.opp_id);
                                if (attrs.uaMaze == "player")
                                    scope.movesHub.server.sendMove(scope.id, scope.opp_id, e.key);
                                if (scope.maze.CurrentPos.Row == scope.maze.End.Row &&
                                    scope.maze.CurrentPos.Col == scope.maze.End.Col)
                                    onWin();
                            });
                        }

                    }
                });
                scope.$watch('solution', function (newVal, oldVal) {
                    if (newVal !== undefined) {
                        // Restart the player's position
                        var pos = { row: scope.maze.CurrentPos.Row, col: scope.maze.CurrentPos.Col };
                        ctx.clearRect(pos.col * cellWidth, pos.row * cellHeight, cellWidth, cellHeight);
                        scope.maze.CurrentPos.Row = scope.maze.Start.Row;
                        scope.maze.CurrentPos.Col = scope.maze.Start.Col;
                        // Cancel the keyboard events.
                        $document.unbind('keydown');
                        var i = 0;
                        var stop = $interval(function () {
                            if (newVal[i] === '0')
                                moveOneStep("ArrowLeft");
                            else if (newVal[i] === '1')
                                moveOneStep("ArrowRight");
                            else if (newVal[i] === '2')
                                moveOneStep("ArrowUp");
                            else if (newVal[i] === '3')
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