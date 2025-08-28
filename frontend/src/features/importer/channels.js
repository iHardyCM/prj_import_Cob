// catálogo SMS/IVR/EMAIL (+ hints)
export const CHANNELS = [
  { value: "sms",   label: "SMS",   hint: "Cabeceras: DNI, TELEFONO, MENSAJE" },
  { value: "ivr",   label: "IVR",   hint: "Cabeceras: DNI, TELEFONO, NOMBRE" },
  { value: "email", label: "EMAIL", hint: "Cabeceras: DNI, CORREO" },
  { value: "bot", label: "BOT", hint: "Cabeceras: DNI, TELEFONO, NOMBRE"},
  { value: "wapi", label: "WAPI", hint: "Cabeceras: DNI, TELEFONO, MENSAJE"}
];

export const templateByChannel = {
  sms:   "DNI,TELEFONO,MENSAJE\n",
  ivr:   "DNI,TELEFONO,NOMBRE\n",
  email: "DNI,CORREO\n",
  bot: "DNI,TELEFONO,NOMBRE\n",
  wapi: "DNI,TELEFONO,MENSAJE\n",
};
