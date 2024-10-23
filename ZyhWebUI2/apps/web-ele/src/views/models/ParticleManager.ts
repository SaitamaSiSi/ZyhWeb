import { Vector } from "./Vector";
import { Particle } from "./Particle";

export class ParticleManager {
  particles: Array<Particle>;
  maxParticleLength: number = 20;
  particleIndex: number = 0;

  constructor() {
    this.particles = [];
    for (let i = 0; i < this.maxParticleLength; i++) {
      this.particles.push(new Particle(new Vector()));
    }
  }

  update(pos: Vector) {
    let i = 0;
    this.particles.forEach((particle) => {
      if (particle.opacity <= 0 ||
        particle.position.y <= 0 ||
        particle.position.x <= 0
      ) {
        particle.reset(pos);
      }
      if (i >= this.particleIndex && i < this.particleIndex + 5) {
        this.particles[i]?.update();
        this.particleIndex++;
      }
      i++;
    });
    if (this.particleIndex >= this.maxParticleLength) {
      this.particleIndex = 0;
    }
  }

  draw(ctx: CanvasRenderingContext2D) {
    this.particles.forEach((particle) => {
      particle.draw(ctx);
    });
  }
}
