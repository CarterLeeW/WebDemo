/**
 * Interface User represents a login entity and is connected to the 
 *  member via the username property
 */
export interface User {
    username: string;
    token: string;
    photoUrl?: string;
    gender?: string;
    knownAs?: string;
    roles: string[];
}