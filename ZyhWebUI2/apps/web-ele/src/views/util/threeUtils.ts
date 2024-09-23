export const rand = (min: number, max: number) => {
  if ( max === undefined ) {
    max = min;
    min = 0;
  }
  return min + ( max - min ) * Math.random();
}
