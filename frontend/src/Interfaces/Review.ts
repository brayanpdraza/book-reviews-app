import {Libro} from './Libro.ts'
import {Usuario} from './Usuario.ts'

export interface Review {
    id: number;
    calificacion: number;
    comentario: string;
    createdAt: Date;
    libro:Libro;
    usuario:Usuario;
  }
