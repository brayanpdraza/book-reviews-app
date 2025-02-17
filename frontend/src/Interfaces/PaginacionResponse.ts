
export interface PaginacionResponse<T> {
  items: T[];
  totalRegistros: number;
  paginaActual: number;
  tamanoPagina: number;
  totalPaginas: number;s
}