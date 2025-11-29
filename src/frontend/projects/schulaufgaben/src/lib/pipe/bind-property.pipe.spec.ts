import { BindPropertyPipe } from './bind-property.pipe';

describe('BindPropertyPipe', () => {
  it('create an instance', () => {
    const pipe = new BindPropertyPipe();
    expect(pipe).toBeTruthy();
  });
});
