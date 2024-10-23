import { Platform } from "./Platform";
import { getRandomInt } from "#/views/util/utils";
import type { PlayerBlock } from "./PlayerBlock";

export class PlatformManager {
  platforms: Array<Platform>;
  lastPlatform: Platform;
  startHeight: number;
  canvasWidth: number;
  canvasHeight: number;

  constructor(canvasWidth: number, canvasHeight: number, startHeight: number) {
    this.platforms = [];
    this.lastPlatform = new Platform(0, 0 , 0, 0);
    this.startHeight = startHeight;
    this.canvasWidth = canvasWidth;
    this.canvasHeight = canvasHeight;
  }

  isIntersectLeft(player: PlayerBlock) {
    const platform = this.platforms.find((p) => p.position.x <= player.position.x && player.position.x < p.position.x + p.width);
    if (platform) {
      const { x, y } = platform.position;
      const prevRightBottomX = player.width + player.position.x;
      const prevRightBottomY = player.height + player.position.y;
      const tx = (x - prevRightBottomX) / platform.velocity.x;
      const ty = (y - prevRightBottomY) / player.velocity.y;
      return ty < tx;
    }
    return false;
  }

  getButtonPos(x: number, w: number, h: number) {
    const platform = this.platforms.find((p) => p.position.x <= (x + w) && (x + w) <= (p.position.x + p.width));
    if (platform) {
      return platform.position.y - h;
    }
    return -1;
  }

  update() {
    // 如果所有平台不能铺满画布
    while (
      !this.platforms.length ||
      this.lastPlatform.position.x < this.canvasWidth
    ) {
      let randHeight = getRandomInt(this.startHeight - 20, this.startHeight + 20);
      if (!this.platforms.length) {
        randHeight = this.startHeight;
      }
      const newPlatform = new Platform(
        this.lastPlatform.position.x + this.lastPlatform.width,
        this.canvasHeight - randHeight,
        getRandomInt(this.canvasWidth / 2, this.canvasWidth),
        randHeight);
      this.lastPlatform = newPlatform;
      this.platforms.push(newPlatform);
    }

    this.platforms.forEach((platform) => {
      platform.update();
    });
    this.platforms = this.platforms.filter((p) => (p.position.x + p.width) > 0);
  }

  draw(ctx: CanvasRenderingContext2D) {
    this.platforms.forEach((p) => p.draw(ctx));
  }
}
