export class SessionExpiredError extends Error {
  constructor(message: string = "Su sesión ha vencido. Debe loguearse de nuevo!") {
    super(message);
    this.name = "SessionExpiredError";
  }
}