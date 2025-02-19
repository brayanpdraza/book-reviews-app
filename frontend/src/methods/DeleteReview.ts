import { ResponseErrorGet } from '../methods/ResponseErrorGet.ts';
import { fetchWithAuth } from '../methods/fetchWithAuth.ts';
import { RequestOptions } from '../Interfaces/RequestOptions.ts';
import { AppContextType } from '../Interfaces/AppContextType.ts';

export const DeleteReview = async (idReview:number, context: AppContextType) => {
    const CtrlNameReview = 'Review';

    const requestOptions: RequestOptions = {
        method: 'DELETE',
    };
    const response = await fetchWithAuth<void>(
        `${context.apiUrl}/${CtrlNameReview}/EliminarReviewPorId/${idReview}`,
        context.token,
        requestOptions,
        undefined,
        context
    );
    if (!response.ok) {
        const errorContent = await ResponseErrorGet(response);
        throw new Error('Error al eliminar la reseña: ' + errorContent);
    }
    window.alert(`Reseña ${idReview} eliminada`);
}