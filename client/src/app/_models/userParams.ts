import { User } from "./user";

/**
 * Acts as the filters when getting members
 */
export class UserParams {
    gender: string;
    minAge = 18;
    maxAge = 99;
    pageNumber = 1;
    PageSize = 5;
    orderBy = 'lastActive';

    constructor(user: User | null) {
        this.gender = user?.gender === 'female' ? 'male' : 'female';
    }
}