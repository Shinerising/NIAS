const convert = (svg: string) => {
  return "data:image/svg+xml;base64," + btoa(svg);
};
export const ImageComputer = convert(`
<svg
height="512"
viewBox="0 0 512 512"
width="512"
xmlns="http://www.w3.org/2000/svg"
>
<g id="flat">
  <path
    d="m376 472h-240l12.809-38.427a14 14 0 0 1 13.282-9.573h187.818a14 14 0 0 1 13.282 9.573z"
    fill="#7496a6"
  />
  <path d="m208 336h96v104h-96z" fill="#3d5966" />
  <path
    d="m256 384a24 24 0 1 1 24-24 24.027 24.027 0 0 1 -24 24z"
    fill="#fff"
  />
  <path d="m24 56h464v296h-464z" fill="#607d8b" />
  <path d="m56 88h400v232h-400z" fill="#42a5f5" />
  <path
    d="m136 472h240a0 0 0 0 1 0 0v9a7 7 0 0 1 -7 7h-226a7 7 0 0 1 -7-7v-9a0 0 0 0 1 0 0z"
    fill="#607d8b"
  />
</g>
</svg>
`);

export const ImageRouter = convert(`
<svg
height="512"
viewBox="0 0 512 512"
width="512"
xmlns="http://www.w3.org/2000/svg"
>
<g id="flat">
  <g fill="#3d5966">
    <path d="m192 312v8h-32v-8-288h24z" />
    <path d="m320 312v8h32v-8-288h-24z" />
    <path d="m96 464h16v32h-16z" />
    <path d="m400 464h16v32h-16z" />
  </g>
  <path d="m56 408h400v64h-400z" fill="#258adb" />
  <path d="m456 408h-304-96l40-88 64-8h192l64 8z" fill="#42a5f5" />
  <path
    d="m40 424a16 16 0 0 1 -16-16v-368l32 24v344a16 16 0 0 1 -16 16z"
    fill="#607d8b"
  />
  <path
    d="m472 424a16 16 0 0 0 16-16v-368l-32 24v344a16 16 0 0 0 16 16z"
    fill="#607d8b"
  />
  <path d="m80 432h32v16h-32z" fill="#e64f39" />
  <path d="m200 368h16v16h-16z" fill="#f5be41" />
  <path d="m232 368h16v16h-16z" fill="#f5be41" />
  <path d="m264 368h16v16h-16z" fill="#f5be41" />
  <path d="m296 368h16v16h-16z" fill="#f5be41" />
</g>
</svg>
`);

export const ImageSwitch = convert(`
<svg
height="512"
viewBox="0 0 512 512"
width="512"
xmlns="http://www.w3.org/2000/svg"
>
<g id="flat">
  <path d="m24 296h464v136h-464z" fill="#607d8b" />
  <path
    d="m488 296h-464l4.8-24 9.6-48 3.2-16 9.6-48 4.8-24h400l4.8 24 9.6 48 3.2 16 9.6 48z"
    fill="#7496a6"
  />
  <path d="m392 328v56h16v16h32v-16h16v-56z" fill="#3d5966" />
  <path d="m296 328v56h16v16h32v-16h16v-56z" fill="#3d5966" />
  <path d="m200 328v56h16v16h32v-16h16v-56z" fill="#3d5966" />
  <path d="m104 328v56h16v16h32v-16h16v-56z" fill="#3d5966" />
  <path d="m56 320h16v16h-16z" fill="#f5be41" />
  <path d="m56 352h16v16h-16z" fill="#f5be41" />
  <path d="m56 384h16v16h-16z" fill="#f5be41" />
  <g fill="#607d8b">
    <path d="m72 224v16h-36.8l3.2-16z" />
    <path d="m64 256v16h-35.2l3.2-16z" />
    <path d="m483.2 272h-35.2v-16h32z" />
    <path d="m80 192v16h-38.4l3.2-16z" />
    <path d="m88 160v16h-40l3.2-16z" />
    <path d="m476.8 240h-36.8v-16h33.6z" />
    <path d="m470.4 208h-38.4v-16h35.2z" />
    <path d="m464 176h-40v-16h36.8z" />
  </g>
</g>
</svg>
`);
