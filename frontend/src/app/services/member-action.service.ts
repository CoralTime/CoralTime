import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { PagedResult, ODataServiceFactory, ODataService } from './odata';
import { MemberAction } from '../models/member-action';

@Injectable()
export class MemberActionsService {
    readonly odata: ODataService<MemberAction>;

    constructor(private odataFactory: ODataServiceFactory) {
        this.odata = this.odataFactory.CreateService<MemberAction>('MemberActions');
    }


    getMemberActions(event, filterStr = ''): Observable<PagedResult<MemberAction>> {
        let filters = [];
        let query = this.odata
            .Query()
            .Top(event.rows)
            .Skip(event.first);

        if (event.sortField) {
            query.OrderBy(event.sortField + ' ' + (event.sortOrder === 1 ? 'desc':'asc'));
        } else {
            query.OrderBy('date' + ' ' + (event.sortOrder === 1 ? 'desc':'asc'));
        }

        if (filterStr) {
            let entityFilter = 'contains(tolower(entity),\'' + filterStr.trim().toLowerCase() + '\')';
            //let memberFilter = 'contains(tolower(memberFullName),\'' + filterStr.trim().toLowerCase() + '\')';
            let changesFilter = 'contains(tolower(changedFields),\'' + filterStr.trim().toLowerCase() + '\')';
            let actionFilter = 'contains(tolower(action),\'' + filterStr.trim().toLowerCase() + '\')';            
            filters.push(entityFilter);
            //filters.push(memberFilter);
            filters.push(changesFilter);
            filters.push(actionFilter);
        }
        query.Filter(filters.join(' or '));

        
        return query.ExecWithCount().map(res => {
            res.data = res.data.map((x: Object) => new MemberAction(x));
            return res;
        });
    }
    
}