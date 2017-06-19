(function ( JQ ) {
 
    JQ.fn.mazeBoard = function(maze) {
 
        // This is the easiest way to have default options.

        JQ(document).on('keydown', keyDownHandler);
        
        var ctx = this[0].getContext('2d');
        var rows = maze.rows;
        var cols = maze.cols;
        var cellWidth = this[0].width / cols;
        var cellHeight = this[0].height / rows;


        for (var i = 0; i < rows; i++)
            for (var j = 0; j < cols; j++) {
                var val = maze.mazeData[(i * cols) + j];
                switch (val) {
                    case '1':
                        ctx.fillRect(cellWidth * j, cellHeight * i, cellWidth, cellHeight);
                        break;
                    case '#':
                        var img1 = new Image();

                        var x1 = cellWidth * j;
                        var y1 = cellHeight * i;

                        img1.onload = function() {
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

        function keyDownHandler(e) {
            var src = maze.pos;
            var dest = undefined;

            if (e.key == "ArrowUp" && maze.pos.row > 0) {
                dest = { row: maze.pos.row - 1, col: maze.pos.col };
            } else if (e.key == "ArrowDown" && maze.pos.row < maze.rows - 1) {
                dest = { row: maze.pos.row + 1, col: maze.pos.col };
            } else if (e.key == "ArrowRight" && maze.pos.col < maze.cols - 1) {
                dest = { row: maze.pos.row, col: maze.pos.col + 1 };
            } else if (e.key == "ArrowLeft" && maze.pos.col > 0) {
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

                if (maze.getValue(maze.pos.row, maze.pos.col) == '#') {
                    onWin();
                }
                
            }
        }

        function onWin() {
            JQ(document).off('keydown', keyDownHandler);
            alert("You win!");
        }

    };
}( jQuery ));