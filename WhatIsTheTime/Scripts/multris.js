var Board = Vue.extend({
    template:
        "<div>" +
        "<table >" +
        " <tr v-for='row in grid'>" +
        "  <td v-for='color in row'>" +
        "    <div class='multris-cell' :class='color'>&nbsp;</div>" +
        "  </td>" +
        " </tr > " +
        "</table > " +
        "<div v-if='done' class='multris-done'>DONE!</div>" +
        "<div>Score: {{score}}</div>" +
        "</div>",
    data: function () {
        return {
            rows: 15,
            colums: 10,
            cells: [],
            currentShape: {
                color: "yellow",
                rotation: 0,
                position: { x: 5, y: 1 }
            },
            done: false,
            score: 0
        };
    },
    computed: {
        grid: function () {
            var result = [];
            for (var r = 0; r < this.rows; r++) {
                var cells = [];
                for (var c = 0; c < this.colums; c++) {
                    cells.push(this.colors[0])
                }
                result.push(cells);
            }
            this.cells.map(c => result[c.y][c.x] = c.color);
            if (this.currentShape) {
                this.cellsForShape(this.currentShape)
                    .filter(c => c.x >= 0 && c.y >= 0 && c.x < this.colums && c.y < this.rows)
                    .map(c => result[c.y][c.x] = this.currentShape.color);
            }

            return result;
        },
        colors: function () {
            return "empty,yellow,green,red,blue".split(",");
        },
        shapes: function () {
            return {
                empty: [],
                yellow: [
                    { x: -1, y: 1 },
                    { x: 0, y: 1 },
                    { x: 1, y: 1 },
                    { x: 1, y: 0 }
                ],
                green: [
                    { x: -1, y: 1 },
                    { x: 0, y: 1 },
                    { x: 0, y: 0 },
                    { x: 1, y: 0 }
                ],
                red: [
                    { x: -1, y: 0 },
                    { x: 0, y: 0 },
                    { x: 1, y: 0 },
                    { x: 2, y: 0 }
                ],
                blue: [
                    { x: -1, y: 0 },
                    { x: -1, y: 1 },
                    { x: 0, y: 1 },
                    { x: 0, y: 0 }
                ],
            }
        },
    },
    methods: {
        areAllCellsEmpty: function (cells) {
            return !cells.some(c => this.cells.some(gc => gc.x == c.x && gc.y == c.y));
        },
        areAllCellsInGrid: function (cells) {
            return cells.every(c => c.x >= 0 && c.y >= 0 && c.x < this.colums && c.y < this.rows);
        },
        rotateCell: function (cell, rotation) {
            switch (rotation & 3) {
                case 0: return { x: cell.x, y: cell.y };
                case 1: return { x: cell.y, y: -cell.x };
                case 2: return { x: -cell.x, y: -cell.y };
                case 3: return { x: -cell.y, y: cell.x };
            }
        },
        moveCell: function (cell, position) {
            return { x: cell.x + position.x, y: cell.y + position.y };
        },
        cellsForShape: function (shape) {
            var cells = this.shapes[shape.color];
            return cells.map(c => this.moveCell(this.rotateCell(c, shape.rotation), shape.position));
        },
        onKeyUp: function () {
            this.currentShape.rotation += 1;
            this.currentShape.rotation &= 3;
        },
        deleteFullLines: function () {
            var rowsToDelete = [];

            for (var y = 0; y < this.rows; y++) {
                var rowEls = this.cells.filter(c => c.y == y);
                if (rowEls.length == this.colums) {
                    rowsToDelete.push(y);
                }
            }
            //TODO: delete lines
            return 0;
        }
    },
    mounted: function () {
        //left = 37
        //up = 38
        //right = 39
        //down = 40


        document.addEventListener("keyup", function (e) {
            var newShape = JSON.parse(JSON.stringify(this.currentShape));

            switch (e.keyCode) {
                case 37: newShape.position.x -= 1;
                    break;
                case 38: newShape.rotation = (newShape.rotation + 1) & 3;
                    break;
                case 39: newShape.position.x += 1;
                    break;
                case 40: newShape.position.y += 1;
                    break;
                default:
                    return;
            }
            e.preventDefault();
            var cells = this.cellsForShape(newShape);
            if (this.areAllCellsInGrid(cells) && this.areAllCellsEmpty(cells)) {
                this.currentShape.position = newShape.position;
                this.currentShape.rotation = newShape.rotation;
            }
        }.bind(this));

        var ctr = 0;

        var animate = function () {
            window.requestAnimationFrame(animate);
            ctr++;
            if (this.done) {
                return;
            }
            if (ctr > 20) {
                ctr = 0;
                var newShape = JSON.parse(JSON.stringify(this.currentShape));
                newShape.position.y += 1;
                var cells = this.cellsForShape(newShape);
                if (this.areAllCellsInGrid(cells) && this.areAllCellsEmpty(cells)) {
                    this.currentShape.position.y += 1;

                } else {
                    cells = this.cellsForShape(this.currentShape);
                    cells.map(c => this.cells.push({ x: c.x, y: c.y, color: this.currentShape.color }));
                    this.score += this.deleteFullLines()*100;

                    this.currentShape.color = this.colors[Math.floor(Math.random() * (this.colors.length - 1)) + 1];
                    console.log(this.currentShape.color)
                    this.currentShape.rotation = 0;
                    this.currentShape.position = { x: 5, y: 1 }
                    cells = this.cellsForShape(this.currentShape);
                    this.done = !this.areAllCellsEmpty(cells);
                    this.score += 1;

                }
            }
        }.bind(this);

        animate();
    }
});

new Vue({
    template:
        "<div>" +
        " <h1>test</h1>" +
        " <board></board>" +
        "</div>",
    components: {
        "board": Board
    }
}).$mount("#multris")

