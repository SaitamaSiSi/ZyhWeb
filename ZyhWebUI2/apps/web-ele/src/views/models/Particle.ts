import { Vector } from "./Vector";
import { getRandomInt } from "#/views/util/utils";

export class Particle {
  // 速度
  velocity: Vector = new Vector(getRandomInt(-20, -2), getRandomInt(2, 12));
  // 位置
  position: Vector;
  // 颜色
  color: string = '#1D80FC';
  // 质量
  mass: number = 1;
  // 透明度
  opacity: number = 1;
  // 宽度
  width: number = 25;
  // 高度
  height: number = 25;


  constructor(position = new Vector()) {
    this.position = position.clone();
    this.position.x -= this.width / 2;
    this.applyForce(
      new Vector(0, -6)
    );
  }

  reset(pos: Vector) {
    this.opacity = 1;
    this.position = pos.clone();
    this.position.x -= this.width / 2;
    this.velocity = new Vector(getRandomInt(-20, -2), getRandomInt(2, 12));
    this.applyForce(
      new Vector(0, -6)
    );
  }

  applyForce(force: Vector) {
    this.velocity.add(force.clone().div(this.mass));
  }

  update() {
    // 每次修改一点透明度，产生逐渐消失的视觉效果
    this.opacity -= 0.01;
    this.position.add(this.velocity);
    this.opacity = Math.max(this.opacity, 0);
  }

  draw(ctx: CanvasRenderingContext2D) {
    ctx.save();
    ctx.globalAlpha = this.opacity;
    ctx.fillStyle = this.color;
    ctx.fillRect(this.position.x, this.position.y, this.width, this.height);
    ctx.restore();
  }
}
