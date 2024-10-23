export class Vector {
  x: number;
  y: number;

  constructor(x = 0, y = 0) {
    this.x = x;
    this.y = y;
  }

  clone() {
    return new Vector(this.x, this.y);
  }

  // 加上另一个向量
  add(vector: Vector) {
    this.x += vector.x;
    this.y += vector.y;
    return this;
  }

  // 除以一个标量，反向不变
  div(scale: number) {
    this.x /= scale;
    this.y /= scale;
    return this;
  }

  // 计算向量的长度
  mag() {
    return Math.sqrt(this.x * this.x + this.y * this.y);
  }

  // 求单位向量
  normalize() {
    const m = this.mag();
    if (m !== 0) {
      return this.div(m);
    }
    return this;
  }
}
