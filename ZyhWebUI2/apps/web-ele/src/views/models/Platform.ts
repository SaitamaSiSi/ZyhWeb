import { Vector } from "./Vector";
import { Rect } from "./Rect";
import { getRandomInt } from "#/views/util/utils";

export class Platform extends Rect {
  // 速度
  velocity: Vector = new Vector(-1, 0);
  // 位置
  position: Vector = new Vector();
  // 颜色
  color: string = '#1D80FC';
  // 质量
  mass: number = 1;
  // 间隔
  intervalWidth: number = 0; // getRandomInt(10, 25);

  constructor(x: number, y: number, w: number, h: number) {
    super(w, h);
    this.position = new Vector(x, y);
  }

  update() {
    this.position.add(this.velocity);
  }

  draw(ctx: CanvasRenderingContext2D) {
    ctx.save();
    ctx.fillStyle = this.color;
    ctx.fillRect(this.position.x, this.position.y, this.width - this.intervalWidth, this.height);
    ctx.restore();
  }
}
