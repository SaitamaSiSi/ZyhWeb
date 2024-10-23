import { Vector } from "./Vector";

export class PlayerBlock {
  // 速度
  velocity: Vector;
  // 位置
  position: Vector;
  // 颜色
  color: string;
  // 质量
  mass: number = 1;
  // 宽度
  width: number = 50;
  // 高度
  height: number = 50;
  // 内宽度
  widthInner: number = 30;
  // 内高度
  heightInner: number = 30;
  // 重力加速度
  G: Vector = new Vector(0, 1);
  // 跳跃力
  jumpPower: Vector = new Vector(0, -1);
  // 是否跳跃
  jump = false;

  constructor(velocity = new Vector(), position = new Vector(), color = '#feca57') {
    this.velocity = velocity;
    this.position = position;
    this.color = color;
  }

  applyForce(force: Vector) {
    this.velocity.add(force.clone().div(this.mass));
  }

  isJumpable() {
    return this.velocity.y == 0;
  }

  update(buttonY: number) {
    if (this.jump && this.position.y > 300) {
      // 跳跃
      this.velocity.add(this.jumpPower);
      this.position.add(this.velocity);
    } else {
      this.jump = false;
      if ((this.position.y + this.velocity.y) < buttonY) {
        // 如果没有落地，则添加重力
        this.velocity.add(this.G);
        this.position.add(this.velocity);
      } else {
        // 落地
        this.velocity.y = 0;
        this.position.y = buttonY;
      }
    }
  }

  draw(ctx: CanvasRenderingContext2D) {
    ctx.save();
    ctx.fillStyle = this.color;
    ctx.fillRect(this.position.x, this.position.y, this.width, this.height);
    ctx.clearRect(
      this.position.x + (this.width - this.widthInner) / 2,
      this.position.y + (this.height - this.heightInner) / 2,
      this.widthInner, this.heightInner);
    ctx.restore();
  }
}
