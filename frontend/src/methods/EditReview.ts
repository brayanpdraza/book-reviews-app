import { ReviewModifiers } from '../Interfaces/ReviewModifiers.ts';
import { ResponseErrorGet } from '../methods/ResponseErrorGet.ts';
import { fetchWithAuth } from '../methods/fetchWithAuth.ts';
import { RequestOptions } from '../Interfaces/RequestOptions.ts';
import { AppContextType } from '../Interfaces/AppContextType.ts';

export const EditReview = async (idReview:number, dataReviewModificable: ReviewModifiers, context:AppContextType) => {
    console.log("Editar reseña:", idReview);
    const CtrlNameReview = 'Review';

    const requestOptions: RequestOptions = {
        method: 'PATCH',
        body: dataReviewModificable,
    };
    const response = await fetchWithAuth<void>(
        `${context.apiUrl}/${CtrlNameReview}/ModificarReviewParcial/${idReview}`,
        context.token,
        requestOptions,
        undefined,
        context
    );

    if (!response.ok) {
        const errorContent = await ResponseErrorGet(response);
        throw new Error('Error al modificar la reseña: ' + errorContent);
    }

    window.alert('Cambios guardados');
}